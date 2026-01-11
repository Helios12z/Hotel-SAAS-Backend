using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreatePaymentDto
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; } = "USD";
        public PaymentMethod Method { get; set; }
        public string? Gateway { get; set; }
        public string? CardLast4Digits { get; set; }
        public string? TransactionId { get; set; }
    }

    // Response DTOs
    public class PaymentDto : BaseDto
    {
        public Guid BookingId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Gateway { get; set; }
        public string? CardLast4Digits { get; set; }
        public string? ReceiptUrl { get; set; }
    }
}
