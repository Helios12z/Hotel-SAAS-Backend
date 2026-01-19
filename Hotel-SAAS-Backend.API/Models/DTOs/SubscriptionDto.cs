using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Subscription Plan DTOs
    public class SubscriptionPlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SubscriptionPlanType PlanType { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal QuarterlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public int MaxHotels { get; set; }
        public int MaxRoomsPerHotel { get; set; }
        public int MaxUsersPerHotel { get; set; }
        public decimal CommissionRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPopular { get; set; }
    }

    public class CreateSubscriptionPlanDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SubscriptionPlanType PlanType { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal QuarterlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public int MaxHotels { get; set; } = 1;
        public int MaxRoomsPerHotel { get; set; } = 50;
        public int MaxUsersPerHotel { get; set; } = 5;
        public decimal CommissionRate { get; set; } = 15;
        public bool IsPopular { get; set; }
        public int SortOrder { get; set; }
    }

    public class UpdateSubscriptionPlanDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public decimal? QuarterlyPrice { get; set; }
        public decimal? YearlyPrice { get; set; }
        public int? MaxHotels { get; set; }
        public int? MaxRoomsPerHotel { get; set; }
        public int? MaxUsersPerHotel { get; set; }
        public decimal? CommissionRate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPopular { get; set; }
        public int? SortOrder { get; set; }
    }

    // Subscription DTOs
    public class SubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public SubscriptionPlanType PlanType { get; set; }
        public SubscriptionStatus Status { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Currency { get; set; } = "USD";
        public bool AutoRenew { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public Guid BrandId { get; set; }
        public Guid PlanId { get; set; }
        public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;
        public decimal DiscountPercentage { get; set; } = 0;
        public bool StartWithTrial { get; set; } = true;
    }

    public class UpdateSubscriptionDto
    {
        public BillingCycle? BillingCycle { get; set; }
        public bool? AutoRenew { get; set; }
    }

    public class CancelSubscriptionDto
    {
        public string? Reason { get; set; }
        public bool CancelImmediately { get; set; } = false;
    }

    public class ChangeSubscriptionPlanDto
    {
        public Guid NewPlanId { get; set; }
        public BillingCycle? NewBillingCycle { get; set; }
    }

    // Invoice DTOs
    public class SubscriptionInvoiceDto
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public InvoiceStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime DueDate { get; set; }
        public string? InvoicePdfUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PayInvoiceDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
    }
}
