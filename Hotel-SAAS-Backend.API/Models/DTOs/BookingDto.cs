using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateBookingDto
    {
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<BookingRoomDto> Rooms { get; set; } = new();
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhoneNumber { get; set; }
        public string? GuestAddress { get; set; }
        public string? GuestNationality { get; set; }
        public string? SpecialRequests { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Currency { get; set; } = "USD";
        public string? CouponCode { get; set; }
    }

    public class BookingRoomDto
    {
        public Guid RoomId { get; set; }
        public int NumberOfAdults { get; set; } = 1;
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfInfants { get; set; } = 0;
        public string? GuestName { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class UpdateBookingDto
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class CalculatePriceDto
    {
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<BookingRoomDto> Rooms { get; set; } = new();
        public string? Currency { get; set; } = "USD";
    }

    // Response DTOs
    public class BookingDto : BaseDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public Guid GuestId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string ConfirmationNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public BookingStatus Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? BookedAt { get; set; }
    }

    public class BookingDetailDto : BookingDto
    {
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? SpecialRequests { get; set; }
        public string? CancellationPolicy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public List<BookingRoomDetailDto> Rooms { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
        public string? HotelImageUrl { get; set; }
        public string? HotelAddress { get; set; }
        public string? HotelCity { get; set; }
        public string? HotelPhoneNumber { get; set; }
        public string? AppliedCouponCode { get; set; }
    }

    public class BookingRoomDetailDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public decimal Price { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public string? GuestName { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class BookingCalculationDto
    {
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public int NumberOfNights { get; set; }
        public List<BookingRoomPriceDto> RoomPrices { get; set; } = new();
    }

    public class BookingRoomPriceDto
    {
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public RoomType RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfNights { get; set; }
    }

    // ============ Additional Charge DTOs ============

    public class AdditionalChargeDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // LateCheckout, Minibar, RoomService, Damages, Other
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAdditionalChargeDto
    {
        public Guid BookingId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }

    // ============ Change Room DTOs ============

    public class ChangeRoomRequestDto
    {
        public Guid OldRoomId { get; set; }
        public Guid NewRoomId { get; set; }
        public string? Reason { get; set; }
    }

    // ============ Late Checkout DTOs ============

    public class LateCheckoutRequestDto
    {
        public DateTime NewCheckOutTime { get; set; } // New check-out date/time
        public string? Reason { get; set; }
    }

    public class LateCheckoutResponseDto
    {
        public decimal LateFeeAmount { get; set; }
        public int ExtraHours { get; set; }
        public decimal HourlyRate { get; set; }
        public DateTime OriginalCheckOut { get; set; }
        public DateTime NewCheckOut { get; set; }
        public decimal OriginalTotal { get; set; }
        public decimal NewTotal { get; set; }
    }

    // ============ Enhanced CheckOut DTOs ============

    public class CheckOutRequestDto
    {
        public List<CreateAdditionalChargeDto>? AdditionalCharges { get; set; }
        public string? PaymentMethod { get; set; } // Cash, Card, VNPay, etc.
        public string? Notes { get; set; }
    }

    public class CheckOutResponseDto
    {
        public Guid BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public decimal RoomCharges { get; set; }
        public decimal AdditionalCharges { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public List<AdditionalChargeDto> Charges { get; set; } = new();
    }

    // ============ Room Status DTOs ============

    public class RoomStatusReportDto
    {
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public RoomStatus CurrentStatus { get; set; }
        public RoomStatus NewStatus { get; set; }
        public string? Reason { get; set; }
    }

    public class RoomMaintenanceReportDto
    {
        public Guid RoomId { get; set; }
        public string Issue { get; set; } = string.Empty; // Plumbing, Electrical, Cleaning, Damages, Other
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
        public string? ReportedBy { get; set; }
    }

    // ============ Available Rooms Filter DTO ============

    public class AvailableRoomsFilterDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public RoomType? Type { get; set; }
        public int? MinOccupancy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool ExcludeMaintenance { get; set; } = true;
        public bool ExcludeCleaning { get; set; } = true;
        public bool ExcludeOutOfOrder { get; set; } = true;
    }
}
