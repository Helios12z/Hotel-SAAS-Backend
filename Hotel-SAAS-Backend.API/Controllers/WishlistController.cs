using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<WishlistSummaryDto>>> GetMyWishlist()
        {
            var result = await _wishlistService.GetUserWishlistAsync(GetUserId());
            return Ok(new ApiResponseDto<WishlistSummaryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("check/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CheckInWishlist(Guid hotelId)
        {
            var result = await _wishlistService.IsInWishlistAsync(GetUserId(), hotelId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<WishlistDto>>> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            try
            {
                var result = await _wishlistService.AddToWishlistAsync(GetUserId(), dto);
                return Ok(new ApiResponseDto<WishlistDto>
                {
                    Success = true,
                    Message = Messages.Misc.AddedToWishlist,
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto<WishlistDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseDto<WishlistDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("toggle/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ToggleWishlist(Guid hotelId)
        {
            try
            {
                var isAdded = await _wishlistService.ToggleWishlistAsync(GetUserId(), hotelId);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = isAdded ? "Added to wishlist" : "Removed from wishlist",
                    Data = isAdded
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = Messages.Hotel.NotFound
                });
            }
        }

        [HttpPut("{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<WishlistDto>>> UpdateNote(
            Guid hotelId,
            [FromBody] UpdateWishlistDto dto)
        {
            var result = await _wishlistService.UpdateNoteAsync(GetUserId(), hotelId, dto);
            if (result == null)
            {
                return NotFound(new ApiResponseDto<WishlistDto>
                {
                    Success = false,
                    Message = Messages.Misc.ItemNotFoundInWishlist
                });
            }

            return Ok(new ApiResponseDto<WishlistDto>
            {
                Success = true,
                Message = Messages.Misc.WishlistNoteUpdated,
                Data = result
            });
        }

        [HttpDelete("{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> RemoveFromWishlist(Guid hotelId)
        {
            var result = await _wishlistService.RemoveFromWishlistAsync(GetUserId(), hotelId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Removed from wishlist" : "Item not found in wishlist",
                Data = result
            });
        }
    }
}
