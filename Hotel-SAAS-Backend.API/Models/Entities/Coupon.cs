using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Coupon : BaseEntity
    {
        public Guid PromotionId { get; set; }
        public string Code { get; set; } = string.Empty;  // Unique coupon code
        
        public CouponStatus Status { get; set; } = CouponStatus.Active;
        
        // Assigned to specific user (optional)
        public Guid? AssignedToUserId { get; set; }
        
        // Usage tracking
        public Guid? UsedByUserId { get; set; }
        public Guid? UsedInBookingId { get; set; }
        public DateTime? UsedAt { get; set; }
        public decimal? DiscountApplied { get; set; }
        
        // Validity override (if different from promotion)
        public DateTime? ExpiresAt { get; set; }
        
        // Navigation
        public virtual Promotion Promotion { get; set; } = null!;
        public virtual User? AssignedToUser { get; set; }
        public virtual User? UsedByUser { get; set; }
        public virtual Booking? UsedInBooking { get; set; }
    }
}
