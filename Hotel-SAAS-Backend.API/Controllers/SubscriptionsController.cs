using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Get all subscriptions (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<SubscriptionDto>>>> GetAllSubscriptions()
        {
            var subscriptions = await _subscriptionService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<SubscriptionDto>>
            {
                Success = true,
                Data = subscriptions
            });
        }

        /// <summary>
        /// Get subscription by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionDto>>> GetSubscriptionById(Guid id)
        {
            var subscription = await _subscriptionService.GetByIdAsync(id);
            if (subscription == null)
            {
                return NotFound(new ApiResponseDto<SubscriptionDto>
                {
                    Success = false,
                    Message = Messages.Subscription.NotFound
                });
            }

            return Ok(new ApiResponseDto<SubscriptionDto>
            {
                Success = true,
                Data = subscription
            });
        }

        /// <summary>
        /// Get active subscription for a brand
        /// </summary>
        [HttpGet("brand/{brandId}/active")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionDto>>> GetActiveByBrand(Guid brandId)
        {
            var subscription = await _subscriptionService.GetActiveByBrandAsync(brandId);
            if (subscription == null)
            {
                return NotFound(new ApiResponseDto<SubscriptionDto>
                {
                    Success = false,
                    Message = Messages.Subscription.ActiveNotFound
                });
            }

            return Ok(new ApiResponseDto<SubscriptionDto>
            {
                Success = true,
                Data = subscription
            });
        }

        /// <summary>
        /// Get all subscriptions for a brand
        /// </summary>
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<SubscriptionDto>>>> GetByBrand(Guid brandId)
        {
            var subscriptions = await _subscriptionService.GetByBrandAsync(brandId);
            return Ok(new ApiResponseDto<IEnumerable<SubscriptionDto>>
            {
                Success = true,
                Data = subscriptions
            });
        }

        /// <summary>
        /// Create a new subscription (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<SubscriptionDto>>> CreateSubscription(
            [FromBody] CreateSubscriptionDto createDto)
        {
            try
            {
                var subscription = await _subscriptionService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetSubscriptionById), new { id = subscription.Id }, new ApiResponseDto<SubscriptionDto>
                {
                    Success = true,
                    Message = Messages.Subscription.Created,
                    Data = subscription
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<SubscriptionDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Update subscription settings
        /// </summary>
        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionDto>>> UpdateSubscription(
            Guid id,
            [FromBody] UpdateSubscriptionDto updateDto)
        {
            try
            {
                var subscription = await _subscriptionService.UpdateAsync(id, updateDto);
                return Ok(new ApiResponseDto<SubscriptionDto>
                {
                    Success = true,
                    Message = Messages.Subscription.Updated,
                    Data = subscription
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<SubscriptionDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Change subscription plan
        /// </summary>
        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPost("{id}/change-plan")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionDto>>> ChangePlan(
            Guid id,
            [FromBody] ChangeSubscriptionPlanDto changeDto)
        {
            try
            {
                var subscription = await _subscriptionService.ChangePlanAsync(id, changeDto);
                return Ok(new ApiResponseDto<SubscriptionDto>
                {
                    Success = true,
                    Message = Messages.Subscription.PlanChanged,
                    Data = subscription
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<SubscriptionDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Cancel subscription
        /// </summary>
        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CancelSubscription(
            Guid id,
            [FromBody] CancelSubscriptionDto cancelDto)
        {
            try
            {
                var result = await _subscriptionService.CancelAsync(id, cancelDto);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? Messages.Subscription.Cancelled : Messages.Subscription.CancelFailed,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Renew subscription
        /// </summary>
        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPost("{id}/renew")]
        public async Task<ActionResult<ApiResponseDto<bool>>> RenewSubscription(Guid id)
        {
            try
            {
                var result = await _subscriptionService.RenewAsync(id);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? Messages.Subscription.Renewed : Messages.Subscription.RenewFailed,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get invoices for a subscription
        /// </summary>
        [HttpGet("{id}/invoices")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<SubscriptionInvoiceDto>>>> GetInvoices(Guid id)
        {
            var invoices = await _subscriptionService.GetInvoicesAsync(id);
            return Ok(new ApiResponseDto<IEnumerable<SubscriptionInvoiceDto>>
            {
                Success = true,
                Data = invoices
            });
        }

        /// <summary>
        /// Get invoice by ID
        /// </summary>
        [HttpGet("invoices/{invoiceId}")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionInvoiceDto>>> GetInvoiceById(Guid invoiceId)
        {
            var invoice = await _subscriptionService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound(new ApiResponseDto<SubscriptionInvoiceDto>
                {
                    Success = false,
                    Message = Messages.Subscription.InvoiceNotFound
                });
            }

            return Ok(new ApiResponseDto<SubscriptionInvoiceDto>
            {
                Success = true,
                Data = invoice
            });
        }

        /// <summary>
        /// Pay an invoice
        /// </summary>
        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPost("invoices/{invoiceId}/pay")]
        public async Task<ActionResult<ApiResponseDto<bool>>> PayInvoice(
            Guid invoiceId,
            [FromBody] PayInvoiceDto payDto)
        {
            try
            {
                var result = await _subscriptionService.PayInvoiceAsync(invoiceId, payDto);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? Messages.Subscription.InvoicePaid : Messages.Subscription.InvoicePayFailed,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Check if brand can add more hotels
        /// </summary>
        [HttpGet("brand/{brandId}/can-add-hotel")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CanAddHotel(Guid brandId)
        {
            var result = await _subscriptionService.CanAddHotelAsync(brandId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = result ? Messages.Subscription.CanAddHotel : Messages.Subscription.PlanLimitReached,
                Data = result
            });
        }

        /// <summary>
        /// Check if hotel can add more rooms
        /// </summary>
        [HttpGet("hotel/{hotelId}/can-add-room")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CanAddRoom(Guid hotelId)
        {
            var result = await _subscriptionService.CanAddRoomAsync(hotelId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = result ? Messages.Subscription.CanAddRoom : Messages.Subscription.PlanLimitReached,
                Data = result
            });
        }

        /// <summary>
        /// Check if hotel can add more users
        /// </summary>
        [HttpGet("hotel/{hotelId}/can-add-user")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CanAddUser(Guid hotelId)
        {
            var result = await _subscriptionService.CanAddUserAsync(hotelId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = result ? Messages.Subscription.CanAddUser : Messages.Subscription.PlanLimitReached,
                Data = result
            });
        }
    }
}
