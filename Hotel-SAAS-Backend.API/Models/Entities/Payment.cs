using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Payment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Currency { get; set; } = "USD";
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime? ProcessedAt { get; set; }
        public string? Gateway { get; set; } // Stripe, PayPal, Bank Transfer
        public string? GatewayTransactionId { get; set; }
        public string? GatewayResponse { get; set; }
        public string? CardLast4Digits { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? InvoiceUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime? RefundedAt { get; set; }
        public decimal? RefundAmount { get; set; }
        public string? RefundReason { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; } = null!;
    }
}
