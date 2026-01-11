using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreatePromotionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;
        public PromotionType Type { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? HotelId { get; set; }
        public int? MinNights { get; set; }
        public int? MinDaysBeforeCheckIn { get; set; }
        public bool IsPublic { get; set; } = true;
    }

    public class UpdatePromotionDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public PromotionStatus? Status { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public int? MinNights { get; set; }
        public int? MinDaysBeforeCheckIn { get; set; }
        public bool? IsPublic { get; set; }
    }

    public class ValidateCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public Guid HotelId { get; set; }
        public decimal BookingAmount { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public class ApplyCouponDto
    {
        public string Code { get; set; } = string.Empty;
    }

    // Response DTOs
    public class PromotionDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;
        public PromotionType Type { get; set; }
        public PromotionStatus Status { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public Guid? HotelId { get; set; }
        public string? HotelName { get; set; }
        public int? MinNights { get; set; }
        public int? MinDaysBeforeCheckIn { get; set; }
        public bool IsPublic { get; set; }
        public bool IsValid { get; set; }  // Computed: còn hi?u l?c không
    }

    public class CouponDto : BaseDto
    {
        public Guid PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public CouponStatus Status { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal? DiscountApplied { get; set; }
        public DateTime? UsedAt { get; set; }
    }

    public class CouponValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Code { get; set; }
        public PromotionType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? CalculatedDiscount { get; set; }
        public decimal? FinalAmount { get; set; }
    }
}
