namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateCheckoutSessionDto
    {
        public Guid BookingId { get; set; }
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }

    public class CreateSubscriptionCheckoutDto
    {
        public Guid SubscriptionId { get; set; }
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }

    // Response DTOs
    public class StripeCheckoutSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public long AmountTotal { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class StripeSessionStatusDto
    {
        public string SessionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public string? CustomerEmail { get; set; }
        public long AmountTotal { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        
        // Metadata
        public Guid? BookingId { get; set; }
        public Guid? SubscriptionId { get; set; }
    }

    public class StripeWebhookEventDto
    {
        public string EventType { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public bool Success { get; set; }
    }
}
