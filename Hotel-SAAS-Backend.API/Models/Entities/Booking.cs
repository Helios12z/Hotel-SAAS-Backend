using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Booking : BaseEntity
    {
        public Guid HotelId { get; set; }
        public Guid GuestId { get; set; }
        public string ConfirmationNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; } = "USD";
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string? SpecialRequests { get; set; }
        public string? GuestNotes { get; set; }
        public DateTime? BookedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public string? Channel { get; set; } // Direct, OTA, Traveloka, Booking.com
        public string? ChannelBookingReference { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhoneNumber { get; set; }
        public string? GuestAddress { get; set; }
        public string? GuestNationality { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }
        
        // Coupon/Promotion tracking
        public string? AppliedCouponCode { get; set; }
        public Guid? AppliedPromotionId { get; set; }

        // Navigation properties
        public virtual Hotel Hotel { get; set; } = null!;
        public virtual User Guest { get; set; } = null!;
        public virtual Promotion? AppliedPromotion { get; set; }
        public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<AdditionalCharge> AdditionalCharges { get; set; } = new List<AdditionalCharge>();
    }
}
