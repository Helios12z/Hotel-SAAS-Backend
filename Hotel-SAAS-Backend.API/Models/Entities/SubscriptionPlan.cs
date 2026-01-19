using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    /// <summary>
    /// Subscription plan defining pricing and features for hotel partners
    /// </summary>
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SubscriptionPlanType PlanType { get; set; } = SubscriptionPlanType.Basic;
        
        // Pricing
        public decimal MonthlyPrice { get; set; }
        public decimal QuarterlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string Currency { get; set; } = "USD";
        
        // Trial
        public int TrialDays { get; set; } = 14;
        
        // Features & Limits
        public int MaxHotels { get; set; } = 1;
        public int MaxRoomsPerHotel { get; set; } = 50;
        public int MaxUsersPerHotel { get; set; } = 5;
        public decimal CommissionRate { get; set; } = 15; // Platform commission percentage
        
        // Status
        public bool IsActive { get; set; } = true;
        public bool IsPopular { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        
        // Navigation properties
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
