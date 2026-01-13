using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PromotionDto>>>> GetPublicPromotions()
        {
            var promotions = await _promotionService.GetPublicPromotionsAsync();
            return Ok(new ApiResponseDto<IEnumerable<PromotionDto>>
            {
                Success = true,
                Data = promotions
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> GetById(Guid id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null)
            {
                return NotFound(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = Messages.Misc.PromotionNotFound
                });
            }

            return Ok(new ApiResponseDto<PromotionDto>
            {
                Success = true,
                Data = promotion
            });
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> GetByCode(string code)
        {
            var promotion = await _promotionService.GetByCodeAsync(code);
            if (promotion == null)
            {
                return NotFound(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = Messages.Misc.PromotionNotFound
                });
            }

            return Ok(new ApiResponseDto<PromotionDto>
            {
                Success = true,
                Data = promotion
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PromotionDto>>>> GetAll()
        {
            var promotions = await _promotionService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<PromotionDto>>
            {
                Success = true,
                Data = promotions
            });
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PromotionDto>>>> GetByHotel(Guid hotelId)
        {
            var promotions = await _promotionService.GetActivePromotionsAsync(hotelId: hotelId);
            return Ok(new ApiResponseDto<IEnumerable<PromotionDto>>
            {
                Success = true,
                Data = promotions
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> Create([FromBody] CreatePromotionDto dto)
        {
            try
            {
                var promotion = await _promotionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = promotion.Id },
                    new ApiResponseDto<PromotionDto>
                    {
                        Success = true,
                        Message = Messages.Misc.PromotionCreated,
                        Data = promotion
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> Update(Guid id, [FromBody] UpdatePromotionDto dto)
        {
            try
            {
                var promotion = await _promotionService.UpdateAsync(id, dto);
                return Ok(new ApiResponseDto<PromotionDto>
                {
                    Success = true,
                    Message = Messages.Misc.PromotionUpdated,
                    Data = promotion
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = Messages.Misc.PromotionNotFound
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/activate")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Activate(Guid id)
        {
            var result = await _promotionService.ActivateAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Promotion activated" : "Failed to activate promotion",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/pause")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Pause(Guid id)
        {
            var result = await _promotionService.PauseAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Promotion paused" : "Failed to pause promotion",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
        {
            await _promotionService.DeleteAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.Misc.PromotionDeleted,
                Data = true
            });
        }

        [Authorize]
        [HttpPost("validate")]
        public async Task<ActionResult<ApiResponseDto<CouponValidationResultDto>>> ValidateCoupon(
            [FromBody] ValidateCouponDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
            
            var result = await _promotionService.ValidateCouponAsync(dto, userId);

            return Ok(new ApiResponseDto<CouponValidationResultDto>
            {
                Success = result.IsValid,
                Message = result.IsValid ? "Coupon is valid" : result.ErrorMessage,
                Data = result
            });
        }
    }
}
