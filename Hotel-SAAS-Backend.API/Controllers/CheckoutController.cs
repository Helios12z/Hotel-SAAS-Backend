using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly StripeOptions _stripeOptions;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            IStripeService stripeService,
            IOptions<StripeOptions> stripeOptions,
            ILogger<CheckoutController> logger)
        {
            _stripeService = stripeService;
            _stripeOptions = stripeOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Get Stripe publishable key for frontend
        /// </summary>
        [HttpGet("config")]
        [AllowAnonymous]
        public ActionResult<object> GetConfig()
        {
            return Ok(new 
            { 
                publishableKey = _stripeOptions.PublishableKey 
            });
        }

        /// <summary>
        /// Create a Stripe checkout session for booking payment
        /// </summary>
        /// <remarks>
        /// Returns a checkout session with URL to redirect user to Stripe hosted checkout page.
        /// 
        /// Test card numbers (Stripe Test Mode):
        /// - Success: 4242 4242 4242 4242
        /// - Declined: 4000 0000 0000 0002
        /// - 3D Secure: 4000 0025 0000 3155
        /// 
        /// Use any future expiry date and any 3-digit CVC.
        /// </remarks>
        [HttpPost("booking")]
        [Authorize]
        public async Task<ActionResult<StripeCheckoutSessionDto>> CreateBookingCheckout(
            [FromBody] CreateCheckoutSessionDto dto)
        {
            try
            {
                var session = await _stripeService.CreateCheckoutSessionAsync(
                    dto.BookingId,
                    dto.SuccessUrl,
                    dto.CancelUrl);

                return Ok(session);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create checkout session for booking {BookingId}", dto.BookingId);
                return StatusCode(500, new { message = "Failed to create checkout session" });
            }
        }

        /// <summary>
        /// Create a Stripe checkout session for subscription payment
        /// </summary>
        [HttpPost("subscription")]
        [Authorize]
        public async Task<ActionResult<StripeCheckoutSessionDto>> CreateSubscriptionCheckout(
            [FromBody] CreateSubscriptionCheckoutDto dto)
        {
            try
            {
                var session = await _stripeService.CreateSubscriptionCheckoutAsync(
                    dto.SubscriptionId,
                    dto.SuccessUrl,
                    dto.CancelUrl);

                return Ok(session);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create checkout session for subscription {SubscriptionId}", dto.SubscriptionId);
                return StatusCode(500, new { message = "Failed to create checkout session" });
            }
        }

        /// <summary>
        /// Get checkout session status
        /// </summary>
        [HttpGet("session/{sessionId}")]
        [Authorize]
        public async Task<ActionResult<StripeSessionStatusDto>> GetSessionStatus(string sessionId)
        {
            var status = await _stripeService.GetSessionStatusAsync(sessionId);
            if (status == null)
                return NotFound(new { message = "Session not found" });

            return Ok(status);
        }

        /// <summary>
        /// Stripe webhook endpoint - called by Stripe to notify payment events
        /// </summary>
        /// <remarks>
        /// This endpoint is called by Stripe when payment events occur.
        /// It must be publicly accessible and configured in Stripe Dashboard.
        /// 
        /// For local testing, use Stripe CLI:
        /// ```
        /// stripe listen --forward-to localhost:5000/api/checkout/webhook
        /// ```
        /// </remarks>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Stripe webhook missing signature");
                return BadRequest(new { message = "Missing Stripe signature" });
            }

            var success = await _stripeService.HandleWebhookAsync(json, signature);
            
            if (!success)
            {
                return BadRequest(new { message = "Webhook processing failed" });
            }

            return Ok();
        }
    }
}
