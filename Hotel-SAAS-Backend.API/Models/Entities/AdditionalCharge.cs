namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class AdditionalCharge : BaseEntity
    {
        public Guid BookingId { get; set; }
        public string Type { get; set; } = string.Empty; // LateCheckout, Minibar, RoomService, Damages, Other
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public Guid? CreatedByUserId { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; } = null!;
    }
}
