using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/guest")]
    [Authorize]
    public class GuestProfileController : ControllerBase
    {
        private readonly IGuestProfileService _guestProfileService;

        public GuestProfileController(IGuestProfileService guestProfileService)
        {
            _guestProfileService = guestProfileService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDto<GuestProfileDto>>> GetProfile()
        {
            var result = await _guestProfileService.GetProfileAsync(GetUserId());
            if (result == null)
            {
                return NotFound(new ApiResponseDto<GuestProfileDto>
                {
                    Success = false,
                    Message = "Profile not found"
                });
            }

            return Ok(new ApiResponseDto<GuestProfileDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPut("preferences")]
        public async Task<ActionResult<ApiResponseDto<GuestProfileDto>>> UpdatePreferences(
            [FromBody] UpdateGuestPreferencesDto dto)
        {
            var result = await _guestProfileService.UpdatePreferencesAsync(GetUserId(), dto);
            if (result == null)
            {
                return NotFound(new ApiResponseDto<GuestProfileDto>
                {
                    Success = false,
                    Message = "Profile not found"
                });
            }

            return Ok(new ApiResponseDto<GuestProfileDto>
            {
                Success = true,
                Message = "Preferences updated",
                Data = result
            });
        }

        [HttpGet("bookings")]
        public async Task<ActionResult<ApiResponseDto<GuestBookingHistoryDto>>> GetBookingHistory(
            [FromQuery] BookingHistoryFilterDto filter)
        {
            var result = await _guestProfileService.GetBookingHistoryAsync(GetUserId(), filter);
            return Ok(new ApiResponseDto<GuestBookingHistoryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("recently-viewed")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<RecentlyViewedHotelDto>>>> GetRecentlyViewed(
            [FromQuery] int limit = 10)
        {
            var result = await _guestProfileService.GetRecentlyViewedAsync(GetUserId(), limit);
            return Ok(new ApiResponseDto<IEnumerable<RecentlyViewedHotelDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpDelete("recently-viewed")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ClearRecentlyViewed()
        {
            await _guestProfileService.ClearRecentlyViewedAsync(GetUserId());
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "History cleared",
                Data = true
            });
        }
    }
}
