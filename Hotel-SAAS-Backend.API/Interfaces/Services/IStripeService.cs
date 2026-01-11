using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IStripeService
    {
        /// <summary>
        /// Create a Stripe Checkout Session for booking payment
        /// </summary>
        Task<StripeCheckoutSessionDto> CreateCheckoutSessionAsync(Guid bookingId, string? successUrl = null, string? cancelUrl = null);
        
        /// <summary>
        /// Create a Stripe Checkout Session for subscription payment
        /// </summary>
        Task<StripeCheckoutSessionDto> CreateSubscriptionCheckoutAsync(Guid subscriptionId, string? successUrl = null, string? cancelUrl = null);
        
        /// <summary>
        /// Retrieve checkout session status
        /// </summary>
        Task<StripeSessionStatusDto?> GetSessionStatusAsync(string sessionId);
        
        /// <summary>
        /// Process webhook event from Stripe
        /// </summary>
        Task<bool> HandleWebhookAsync(string json, string signature);
        
        /// <summary>
        /// Refund a payment
        /// </summary>
        Task<bool> RefundPaymentAsync(string paymentIntentId, long? amountInCents = null, string? reason = null);
    }
}
