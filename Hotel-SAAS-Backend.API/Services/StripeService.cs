using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Options;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using LocalEnums = Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Models.Constants;

namespace Hotel_SAAS_Backend.API.Services
{
    public class StripeService : IStripeService
    {
        private readonly StripeOptions _options;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionInvoiceRepository _invoiceRepository;
        private readonly ILogger<StripeService> _logger;

        public StripeService(
            IOptions<StripeOptions> options,
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionInvoiceRepository invoiceRepository,
            ILogger<StripeService> logger)
        {
            _options = options.Value;
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _invoiceRepository = invoiceRepository;
            _logger = logger;

            // Configure Stripe
            StripeConfiguration.ApiKey = _options.SecretKey;
        }

        public async Task<StripeCheckoutSessionDto> CreateCheckoutSessionAsync(
            Guid bookingId, 
            string? successUrl = null, 
            string? cancelUrl = null)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null)
                throw new ArgumentException(Messages.Subscription.BookingNotFound, nameof(bookingId));

            var baseUrl = _options.FrontendBaseUrl;
            successUrl ??= $"{baseUrl}/bookings/{bookingId}/success?session_id={{CHECKOUT_SESSION_ID}}";
            cancelUrl ??= $"{baseUrl}/bookings/{bookingId}/cancel";

            // Convert amount to cents (Stripe uses smallest currency unit)
            var amountInCents = (long)(booking.TotalAmount * 100);

            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = _options.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Booking at {booking.Hotel?.Name ?? "Hotel"}",
                                Description = $"Check-in: {booking.CheckInDate:MMM dd, yyyy} - Check-out: {booking.CheckOutDate:MMM dd, yyyy}",
                                Images = booking.Hotel?.ImageUrl != null ? [booking.Hotel.ImageUrl] : null
                            },
                            UnitAmount = amountInCents
                        },
                        Quantity = 1
                    }
                ],
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                CustomerEmail = booking.GuestEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId.ToString() },
                    { "confirmation_number", booking.ConfirmationNumber }
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions);

            _logger.LogInformation("Created Stripe checkout session {SessionId} for booking {BookingId}", 
                session.Id, bookingId);

            return new StripeCheckoutSessionDto
            {
                SessionId = session.Id,
                Url = session.Url,
                Status = session.Status,
                PaymentIntentId = session.PaymentIntentId,
                AmountTotal = session.AmountTotal ?? 0,
                Currency = session.Currency,
                ExpiresAt = session.ExpiresAt
            };
        }

        public async Task<StripeCheckoutSessionDto> CreateSubscriptionCheckoutAsync(
            Guid subscriptionId, 
            string? successUrl = null, 
            string? cancelUrl = null)
        {
            var subscription = await _subscriptionRepository.GetByIdWithDetailsAsync(subscriptionId);
            if (subscription == null)
                throw new ArgumentException(Messages.Subscription.NotFound, nameof(subscriptionId));

            var baseUrl = _options.FrontendBaseUrl;
            successUrl ??= $"{baseUrl}/subscriptions/{subscriptionId}/success?session_id={{CHECKOUT_SESSION_ID}}";
            cancelUrl ??= $"{baseUrl}/subscriptions/{subscriptionId}/cancel";

            var amountInCents = (long)(subscription.Price * 100);
            var planName = subscription.Plan?.Name ?? "Subscription Plan";

            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = _options.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{planName} Subscription",
                                Description = $"Billing cycle: {subscription.BillingCycle}"
                            },
                            UnitAmount = amountInCents
                        },
                        Quantity = 1
                    }
                ],
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "subscription_id", subscriptionId.ToString() },
                    { "brand_id", subscription.BrandId.ToString() }
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions);

            _logger.LogInformation("Created Stripe checkout session {SessionId} for subscription {SubscriptionId}", 
                session.Id, subscriptionId);

            return new StripeCheckoutSessionDto
            {
                SessionId = session.Id,
                Url = session.Url,
                Status = session.Status,
                PaymentIntentId = session.PaymentIntentId,
                AmountTotal = session.AmountTotal ?? 0,
                Currency = session.Currency,
                ExpiresAt = session.ExpiresAt
            };
        }

        public async Task<StripeSessionStatusDto?> GetSessionStatusAsync(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId, new SessionGetOptions
                {
                    Expand = ["payment_intent"]
                });

                Guid.TryParse(session.Metadata.GetValueOrDefault("booking_id"), out var bookingId);
                Guid.TryParse(session.Metadata.GetValueOrDefault("subscription_id"), out var subscriptionId);

                return new StripeSessionStatusDto
                {
                    SessionId = session.Id,
                    Status = session.Status,
                    PaymentStatus = session.PaymentStatus,
                    PaymentIntentId = session.PaymentIntentId,
                    CustomerEmail = session.CustomerEmail,
                    AmountTotal = session.AmountTotal ?? 0,
                    Currency = session.Currency,
                    PaidAt = session.PaymentIntent?.Created,
                    BookingId = bookingId == Guid.Empty ? null : bookingId,
                    SubscriptionId = subscriptionId == Guid.Empty ? null : subscriptionId
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to get Stripe session {SessionId}", sessionId);
                return null;
            }
        }

        public async Task<bool> HandleWebhookAsync(string json, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _options.WebhookSecret
                );

                _logger.LogInformation("Processing Stripe webhook event: {EventType}", stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        break;
                    
                    case "checkout.session.expired":
                        await HandleCheckoutSessionExpired(stripeEvent);
                        break;
                    
                    case "payment_intent.succeeded":
                        _logger.LogInformation("Payment intent succeeded");
                        break;
                    
                    case "payment_intent.payment_failed":
                        await HandlePaymentFailed(stripeEvent);
                        break;
                    
                    default:
                        _logger.LogInformation("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed");
                return false;
            }
        }

        public async Task<bool> RefundPaymentAsync(string paymentIntentId, long? amountInCents = null, string? reason = null)
        {
            try
            {
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Amount = amountInCents,
                    Reason = reason switch
                    {
                        "duplicate" => "duplicate",
                        "fraudulent" => "fraudulent",
                        _ => "requested_by_customer"
                    }
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(refundOptions);

                _logger.LogInformation("Created refund {RefundId} for payment intent {PaymentIntentId}", 
                    refund.Id, paymentIntentId);

                return refund.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to refund payment intent {PaymentIntentId}", paymentIntentId);
                return false;
            }
        }

        #region Private Webhook Handlers

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            // Check if this is a booking payment
            if (session.Metadata.TryGetValue("booking_id", out var bookingIdStr) && 
                Guid.TryParse(bookingIdStr, out var bookingId))
            {
                await ProcessBookingPayment(bookingId, session);
            }
            // Check if this is a subscription payment
            else if (session.Metadata.TryGetValue("subscription_id", out var subscriptionIdStr) && 
                     Guid.TryParse(subscriptionIdStr, out var subscriptionId))
            {
                await ProcessSubscriptionPayment(subscriptionId, session);
            }
        }

        private async Task ProcessBookingPayment(Guid bookingId, Session session)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) return;

            // Create payment record
            var payment = new Models.Entities.Payment
            {
                BookingId = bookingId,
                TransactionId = session.PaymentIntentId ?? session.Id,
                Amount = (decimal)(session.AmountTotal ?? 0) / 100,
                Currency = session.Currency?.ToUpper() ?? "USD",
                Method = LocalEnums.PaymentMethod.CreditCard,
                Status = LocalEnums.PaymentStatus.Completed,
                Gateway = "Stripe",
                GatewayTransactionId = session.PaymentIntentId,
                GatewayResponse = session.Id,
                ProcessedAt = DateTime.UtcNow,
                ReceiptUrl = session.PaymentIntent?.ReceiptEmail
            };
            await _paymentRepository.CreateAsync(payment);

            // Update booking status
            booking.IsPaid = true;
            booking.PaidAt = DateTime.UtcNow;
            booking.PaymentMethod = "Stripe";
            booking.Status = LocalEnums.BookingStatus.Confirmed;
            booking.ConfirmedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            _logger.LogInformation("Processed payment for booking {BookingId}, PaymentIntent: {PaymentIntentId}", 
                bookingId, session.PaymentIntentId);
        }

        private async Task ProcessSubscriptionPayment(Guid subscriptionId, Session session)
        {
            var subscription = await _subscriptionRepository.GetByIdWithDetailsAsync(subscriptionId);
            if (subscription == null) return;

            // Create invoice record
            var invoice = new Models.Entities.SubscriptionInvoice
            {
                SubscriptionId = subscriptionId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                PeriodStart = subscription.StartDate,
                PeriodEnd = subscription.EndDate,
                Subtotal = subscription.Price,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = subscription.Price,
                Currency = session.Currency?.ToUpper() ?? "USD",
                Status = LocalEnums.InvoiceStatus.Paid,
                DueDate = DateTime.UtcNow,
                PaidAt = DateTime.UtcNow,
                TransactionId = session.PaymentIntentId,
                PaymentMethod = LocalEnums.PaymentMethod.CreditCard
            };
            await _invoiceRepository.CreateAsync(invoice);

            // Activate subscription
            subscription.Status = LocalEnums.SubscriptionStatus.Active;
            await _subscriptionRepository.UpdateAsync(subscription);

            _logger.LogInformation("Processed payment for subscription {SubscriptionId}, PaymentIntent: {PaymentIntentId}", 
                subscriptionId, session.PaymentIntentId);
        }

        private async Task HandleCheckoutSessionExpired(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            _logger.LogWarning("Checkout session expired: {SessionId}", session.Id);
            
            // Optionally update booking status to expired/pending
            if (session.Metadata.TryGetValue("booking_id", out var bookingIdStr) && 
                Guid.TryParse(bookingIdStr, out var bookingId))
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
                if (booking != null && booking.Status == LocalEnums.BookingStatus.Pending)
                {
                    // Keep as pending, user can try again
                    _logger.LogInformation("Booking {BookingId} checkout session expired, keeping as pending", bookingId);
                }
            }
        }

        private async Task HandlePaymentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            _logger.LogWarning("Payment failed for PaymentIntent: {PaymentIntentId}", paymentIntent.Id);
            
            // Could update payment record status to failed if needed
        }

        #endregion
    }
}
