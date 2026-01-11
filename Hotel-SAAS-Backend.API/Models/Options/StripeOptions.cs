namespace Hotel_SAAS_Backend.API.Models.Options
{
    public class StripeOptions
    {
        public const string SectionName = "Stripe";
        
        /// <summary>
        /// Stripe Secret Key (starts with sk_test_ for test mode)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Stripe Publishable Key (starts with pk_test_ for test mode)
        /// </summary>
        public string PublishableKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Webhook Secret for verifying webhook signatures
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;
        
        /// <summary>
        /// Base URL for success/cancel redirects
        /// </summary>
        public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
        
        /// <summary>
        /// Default currency (ISO 4217 code)
        /// </summary>
        public string Currency { get; set; } = "usd";
    }
}
