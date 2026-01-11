using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/subscription-plans")]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _planService;

        public SubscriptionPlansController(ISubscriptionPlanService planService)
        {
            _planService = planService;
        }

        /// <summary>
        /// Get all active subscription plans (public)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<SubscriptionPlanDto>>>> GetActivePlans()
        {
            var plans = await _planService.GetActiveAsync();
            return Ok(new ApiResponseDto<IEnumerable<SubscriptionPlanDto>>
            {
                Success = true,
                Data = plans
            });
        }

        /// <summary>
        /// Get subscription plan by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionPlanDto>>> GetPlanById(Guid id)
        {
            var plan = await _planService.GetByIdAsync(id);
            if (plan == null)
            {
                return NotFound(new ApiResponseDto<SubscriptionPlanDto>
                {
                    Success = false,
                    Message = "Subscription plan not found"
                });
            }

            return Ok(new ApiResponseDto<SubscriptionPlanDto>
            {
                Success = true,
                Data = plan
            });
        }

        /// <summary>
        /// Get all subscription plans including inactive (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<SubscriptionPlanDto>>>> GetAllPlans()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<SubscriptionPlanDto>>
            {
                Success = true,
                Data = plans
            });
        }

        /// <summary>
        /// Create a new subscription plan (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<SubscriptionPlanDto>>> CreatePlan(
            [FromBody] CreateSubscriptionPlanDto createDto)
        {
            try
            {
                var plan = await _planService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPlanById), new { id = plan.Id }, new ApiResponseDto<SubscriptionPlanDto>
                {
                    Success = true,
                    Message = "Subscription plan created successfully",
                    Data = plan
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<SubscriptionPlanDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Update a subscription plan (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<SubscriptionPlanDto>>> UpdatePlan(
            Guid id,
            [FromBody] UpdateSubscriptionPlanDto updateDto)
        {
            try
            {
                var plan = await _planService.UpdateAsync(id, updateDto);
                return Ok(new ApiResponseDto<SubscriptionPlanDto>
                {
                    Success = true,
                    Message = "Subscription plan updated successfully",
                    Data = plan
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<SubscriptionPlanDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Toggle subscription plan active status (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ToggleActive(Guid id)
        {
            try
            {
                var isActive = await _planService.ToggleActiveAsync(id);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = isActive ? "Plan activated" : "Plan deactivated",
                    Data = isActive
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
        /// Delete a subscription plan (admin only)
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeletePlan(Guid id)
        {
            var result = await _planService.DeleteAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Subscription plan deleted successfully" : "Failed to delete subscription plan",
                Data = result
            });
        }
    }
}
