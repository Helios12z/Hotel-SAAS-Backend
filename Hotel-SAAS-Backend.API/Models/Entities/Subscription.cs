using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    /// <summary>
    /// Brand subscription to a plan
    /// </summary>
    public class Subscription : BaseEntity
    {
        public Guid BrandId { get; set; }
        public Guid PlanId { get; set; }
        
        // Subscription Details
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;
        public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        
        // Pricing at time of subscription (may differ from plan due to discounts)
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public string Currency { get; set; } = "USD";
        
        // Auto Renewal
        public bool AutoRenew { get; set; } = true;
        public DateTime? NextBillingDate { get; set; }
        
        // Navigation properties
        public virtual Brand Brand { get; set; } = null!;
        public virtual SubscriptionPlan Plan { get; set; } = null!;
        public virtual ICollection<SubscriptionInvoice> Invoices { get; set; } = new List<SubscriptionInvoice>();
    }
}
