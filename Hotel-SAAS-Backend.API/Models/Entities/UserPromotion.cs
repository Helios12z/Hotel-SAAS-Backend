namespace Hotel_SAAS_Backend.API.Models.Entities
{
    // Tracking promotion usage per user (không c?n coupon riêng)
    public class UserPromotion : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PromotionId { get; set; }
        public int UsageCount { get; set; } = 0;
        public DateTime LastUsedAt { get; set; }
        
        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Promotion Promotion { get; set; } = null!;
    }
}
