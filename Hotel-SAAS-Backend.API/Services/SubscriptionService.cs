using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPlanRepository planRepository,
        ISubscriptionInvoiceRepository invoiceRepository,
        IBrandRepository brandRepository,
        IHotelRepository hotelRepository,
        IUserRepository userRepository) : ISubscriptionService
    {
        public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(id);
            return subscription == null ? null : MapToDto(subscription);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            var subscriptions = await subscriptionRepository.GetAllAsync();
            return subscriptions.Select(MapToDto);
        }

        public async Task<SubscriptionDto?> GetActiveByBrandAsync(Guid brandId)
        {
            var subscription = await subscriptionRepository.GetActiveByBrandAsync(brandId);
            return subscription == null ? null : MapToDto(subscription);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetByBrandAsync(Guid brandId)
        {
            var subscriptions = await subscriptionRepository.GetByBrandAsync(brandId);
            return subscriptions.Select(MapToDto);
        }

        public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto)
        {
            var plan = await planRepository.GetByIdAsync(dto.PlanId) 
                ?? throw new Exception("Subscription plan not found");

            var brand = await brandRepository.GetByIdAsync(dto.BrandId) 
                ?? throw new Exception("Brand not found");

            if (await subscriptionRepository.HasActiveSubscriptionAsync(dto.BrandId))
                throw new Exception("Brand already has an active subscription");

            var price = GetPriceByBillingCycle(plan, dto.BillingCycle);
            var discountedPrice = price * (1 - dto.DiscountPercentage / 100);

            var now = DateTime.UtcNow;
            var startDate = now;
            var endDate = CalculateEndDate(now, dto.BillingCycle);

            var subscription = new Subscription
            {
                BrandId = dto.BrandId,
                PlanId = dto.PlanId,
                Status = dto.StartWithTrial ? SubscriptionStatus.Trial : SubscriptionStatus.Active,
                BillingCycle = dto.BillingCycle,
                StartDate = startDate,
                EndDate = endDate,
                TrialEndDate = dto.StartWithTrial ? now.AddDays(plan.TrialDays) : null,
                Price = discountedPrice,
                DiscountPercentage = dto.DiscountPercentage,
                Currency = plan.Currency,
                AutoRenew = true,
                NextBillingDate = dto.StartWithTrial ? now.AddDays(plan.TrialDays) : endDate
            };

            var created = await subscriptionRepository.CreateAsync(subscription);

            // Create first invoice if not trial
            if (!dto.StartWithTrial)
            {
                await CreateInvoiceAsync(created.Id);
            }

            return MapToDto(await subscriptionRepository.GetByIdAsync(created.Id)!);
        }

        public async Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto dto)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(id) 
                ?? throw new Exception("Subscription not found");

            if (dto.BillingCycle.HasValue)
            {
                subscription.BillingCycle = dto.BillingCycle.Value;
                var plan = await planRepository.GetByIdAsync(subscription.PlanId);
                if (plan != null)
                {
                    subscription.Price = GetPriceByBillingCycle(plan, dto.BillingCycle.Value) 
                        * (1 - subscription.DiscountPercentage / 100);
                }
            }

            if (dto.AutoRenew.HasValue)
                subscription.AutoRenew = dto.AutoRenew.Value;

            subscription.UpdatedAt = DateTime.UtcNow;

            var updated = await subscriptionRepository.UpdateAsync(subscription);
            return MapToDto(updated);
        }

        public async Task<SubscriptionDto> ChangePlanAsync(Guid id, ChangeSubscriptionPlanDto dto)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(id) 
                ?? throw new Exception("Subscription not found");

            var newPlan = await planRepository.GetByIdAsync(dto.NewPlanId) 
                ?? throw new Exception("New subscription plan not found");

            var billingCycle = dto.NewBillingCycle ?? subscription.BillingCycle;
            var newPrice = GetPriceByBillingCycle(newPlan, billingCycle);

            subscription.PlanId = dto.NewPlanId;
            subscription.BillingCycle = billingCycle;
            subscription.Price = newPrice * (1 - subscription.DiscountPercentage / 100);
            subscription.UpdatedAt = DateTime.UtcNow;

            var updated = await subscriptionRepository.UpdateAsync(subscription);
            return MapToDto(updated);
        }

        public async Task<bool> CancelAsync(Guid id, CancelSubscriptionDto dto)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(id) 
                ?? throw new Exception("Subscription not found");

            subscription.CancellationReason = dto.Reason;
            subscription.CancelledAt = DateTime.UtcNow;
            subscription.AutoRenew = false;

            if (dto.CancelImmediately)
            {
                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.EndDate = DateTime.UtcNow;
            }

            subscription.UpdatedAt = DateTime.UtcNow;

            await subscriptionRepository.UpdateAsync(subscription);
            return true;
        }

        public async Task<bool> RenewAsync(Guid id)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(id) 
                ?? throw new Exception("Subscription not found");

            var plan = await planRepository.GetByIdAsync(subscription.PlanId);
            if (plan == null) throw new Exception("Subscription plan not found");

            subscription.StartDate = DateTime.UtcNow;
            subscription.EndDate = CalculateEndDate(DateTime.UtcNow, subscription.BillingCycle);
            subscription.NextBillingDate = subscription.EndDate;
            subscription.Status = SubscriptionStatus.Active;
            subscription.CancelledAt = null;
            subscription.CancellationReason = null;
            subscription.UpdatedAt = DateTime.UtcNow;

            await subscriptionRepository.UpdateAsync(subscription);

            // Create renewal invoice
            await CreateInvoiceAsync(subscription.Id);

            return true;
        }

        // Invoice operations
        public async Task<IEnumerable<SubscriptionInvoiceDto>> GetInvoicesAsync(Guid subscriptionId)
        {
            var invoices = await invoiceRepository.GetBySubscriptionAsync(subscriptionId);
            return invoices.Select(MapInvoiceToDto);
        }

        public async Task<SubscriptionInvoiceDto?> GetInvoiceByIdAsync(Guid invoiceId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId);
            return invoice == null ? null : MapInvoiceToDto(invoice);
        }

        public async Task<SubscriptionInvoiceDto> CreateInvoiceAsync(Guid subscriptionId)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(subscriptionId) 
                ?? throw new Exception("Subscription not found");

            var invoiceNumber = await invoiceRepository.GenerateInvoiceNumberAsync();
            var taxRate = 0.1m; // 10% tax, should be configurable

            var invoice = new SubscriptionInvoice
            {
                SubscriptionId = subscriptionId,
                InvoiceNumber = invoiceNumber,
                PeriodStart = subscription.StartDate,
                PeriodEnd = subscription.EndDate,
                Subtotal = subscription.Price,
                TaxAmount = subscription.Price * taxRate,
                DiscountAmount = 0,
                TotalAmount = subscription.Price * (1 + taxRate),
                Currency = subscription.Currency,
                Status = InvoiceStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var created = await invoiceRepository.CreateAsync(invoice);
            return MapInvoiceToDto(created);
        }

        public async Task<bool> PayInvoiceAsync(Guid invoiceId, PayInvoiceDto dto)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId) 
                ?? throw new Exception("Invoice not found");

            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = DateTime.UtcNow;
            invoice.PaymentMethod = dto.PaymentMethod;
            invoice.TransactionId = dto.TransactionId;
            invoice.UpdatedAt = DateTime.UtcNow;

            await invoiceRepository.UpdateAsync(invoice);

            // Activate subscription if it was trial or past due
            var subscription = await subscriptionRepository.GetByIdAsync(invoice.SubscriptionId);
            if (subscription != null && 
                (subscription.Status == SubscriptionStatus.Trial || subscription.Status == SubscriptionStatus.PastDue))
            {
                subscription.Status = SubscriptionStatus.Active;
                subscription.UpdatedAt = DateTime.UtcNow;
                await subscriptionRepository.UpdateAsync(subscription);
            }

            return true;
        }

        // Usage & Limits
        public async Task<bool> CanAddHotelAsync(Guid brandId)
        {
            var subscription = await subscriptionRepository.GetActiveByBrandAsync(brandId);
            if (subscription == null) return false;

            var hotels = await hotelRepository.GetByBrandAsync(brandId);
            return hotels.Count() < subscription.Plan.MaxHotels;
        }

        public async Task<bool> CanAddRoomAsync(Guid hotelId)
        {
            var hotel = await hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null) return false;

            var subscription = await subscriptionRepository.GetActiveByBrandAsync(hotel.BrandId);
            if (subscription == null) return false;

            return hotel.Rooms.Count < subscription.Plan.MaxRoomsPerHotel;
        }

        public async Task<bool> CanAddUserAsync(Guid hotelId)
        {
            var hotel = await hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null) return false;

            var subscription = await subscriptionRepository.GetActiveByBrandAsync(hotel.BrandId);
            if (subscription == null) return false;

            var users = await userRepository.GetByHotelAsync(hotelId);
            return users.Count() < subscription.Plan.MaxUsersPerHotel;
        }

        private static decimal GetPriceByBillingCycle(SubscriptionPlan plan, BillingCycle cycle)
        {
            return cycle switch
            {
                BillingCycle.Monthly => plan.MonthlyPrice,
                BillingCycle.Quarterly => plan.QuarterlyPrice,
                BillingCycle.Yearly => plan.YearlyPrice,
                _ => plan.MonthlyPrice
            };
        }

        private static DateTime CalculateEndDate(DateTime startDate, BillingCycle cycle)
        {
            return cycle switch
            {
                BillingCycle.Monthly => startDate.AddMonths(1),
                BillingCycle.Quarterly => startDate.AddMonths(3),
                BillingCycle.Yearly => startDate.AddYears(1),
                _ => startDate.AddMonths(1)
            };
        }

        private static SubscriptionDto MapToDto(Subscription s) => new()
        {
            Id = s.Id,
            BrandId = s.BrandId,
            BrandName = s.Brand?.Name ?? "",
            PlanId = s.PlanId,
            PlanName = s.Plan?.Name ?? "",
            PlanType = s.Plan?.PlanType ?? SubscriptionPlanType.Basic,
            Status = s.Status,
            BillingCycle = s.BillingCycle,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            TrialEndDate = s.TrialEndDate,
            Price = s.Price,
            DiscountPercentage = s.DiscountPercentage,
            Currency = s.Currency,
            AutoRenew = s.AutoRenew,
            NextBillingDate = s.NextBillingDate,
            CreatedAt = s.CreatedAt
        };

        private static SubscriptionInvoiceDto MapInvoiceToDto(SubscriptionInvoice i) => new()
        {
            Id = i.Id,
            SubscriptionId = i.SubscriptionId,
            InvoiceNumber = i.InvoiceNumber,
            PeriodStart = i.PeriodStart,
            PeriodEnd = i.PeriodEnd,
            Subtotal = i.Subtotal,
            TaxAmount = i.TaxAmount,
            DiscountAmount = i.DiscountAmount,
            TotalAmount = i.TotalAmount,
            Currency = i.Currency,
            Status = i.Status,
            PaidAt = i.PaidAt,
            PaymentMethod = i.PaymentMethod,
            DueDate = i.DueDate,
            InvoicePdfUrl = i.InvoicePdfUrl,
            CreatedAt = i.CreatedAt
        };
    }
}
