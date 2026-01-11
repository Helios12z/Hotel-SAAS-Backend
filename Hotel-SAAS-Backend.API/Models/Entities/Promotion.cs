using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Promotion : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;  // Unique code
        
        public PromotionType Type { get; set; }
        public PromotionStatus Status { get; set; } = PromotionStatus.Draft;
        
        // Discount value
        public decimal DiscountValue { get; set; }        // % ho?c s? ti?n
        public decimal? MaxDiscountAmount { get; set; }   // Gi?i h?n max discount cho %
        public decimal? MinBookingAmount { get; set; }    // ??n t?i thi?u
        
        // Validity
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Usage limits
        public int? MaxUsageCount { get; set; }           // T?ng s? l?n s? d?ng
        public int CurrentUsageCount { get; set; } = 0;
        public int? MaxUsagePerUser { get; set; }         // Gi?i h?n m?i user
        
        // Scope: null = t?t c?, có giá tr? = ch? áp d?ng cho brand/hotel ?ó
        public Guid? BrandId { get; set; }
        public Guid? HotelId { get; set; }
        
        // Conditions
        public int? MinNights { get; set; }               // S? ?êm t?i thi?u
        public int? MinDaysBeforeCheckIn { get; set; }    // ??t tr??c bao nhiêu ngày (EarlyBird)
        
        public bool IsPublic { get; set; } = true;        // Hi?n th? công khai hay private
        
        // Navigation
        public virtual Brand? Brand { get; set; }
        public virtual Hotel? Hotel { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    }
}
