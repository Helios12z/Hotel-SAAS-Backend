using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    /// <summary>
    /// Invoice for subscription payment
    /// </summary>
    public class SubscriptionInvoice : BaseEntity
    {
        public Guid SubscriptionId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        
        // Period
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        
        // Amounts
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        
        // Payment
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
        public DateTime? PaidAt { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        
        // Due Date
        public DateTime DueDate { get; set; }
        
        // PDF
        public string? InvoicePdfUrl { get; set; }
        
        // Notes
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Subscription Subscription { get; set; } = null!;
    }
}
