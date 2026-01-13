using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelId}/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ReviewDto>>>> GetReviews(Guid hotelId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var reviews = await _reviewService.GetReviewsByHotelAsync(hotelId, page, pageSize);
            return Ok(new ApiResponseDto<IEnumerable<ReviewDto>>
            {
                Success = true,
                Data = reviews
            });
        }

        [Authorize]
        [HttpGet("my-reviews")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ReviewDto>>>> GetMyReviews()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reviews = await _reviewService.GetReviewsByGuestAsync(userId);
            return Ok(new ApiResponseDto<IEnumerable<ReviewDto>>
            {
                Success = true,
                Data = reviews
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ReviewDto>>> CreateReview(Guid hotelId, [FromBody] CreateReviewDto createReviewDto)
        {
            try
            {
                createReviewDto.HotelId = hotelId;
                var review = await _reviewService.CreateReviewAsync(createReviewDto);
                return Ok(new ApiResponseDto<ReviewDto>
                {
                    Success = true,
                    Message = Messages.Misc.ReviewSubmitted,
                    Data = review
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<ReviewDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<ReviewDto>>> UpdateReview(Guid hotelId, Guid id, [FromBody] UpdateReviewDto updateReviewDto)
        {
            try
            {
                var review = await _reviewService.UpdateReviewAsync(id, updateReviewDto);
                if (review == null)
                {
                    return NotFound(new ApiResponseDto<ReviewDto>
                    {
                        Success = false,
                        Message = Messages.Misc.ReviewNotFound
                    });
                }

                return Ok(new ApiResponseDto<ReviewDto>
                {
                    Success = true,
                    Message = Messages.Misc.ReviewUpdated,
                    Data = review
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<ReviewDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteReview(Guid hotelId, Guid id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Review deleted successfully" : "Failed to delete review",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ApproveReview(Guid hotelId, Guid id)
        {
            var result = await _reviewService.ApproveReviewAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Review approved" : "Failed to approve review",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/reject")]
        public async Task<ActionResult<ApiResponseDto<bool>>> RejectReview(Guid hotelId, Guid id)
        {
            var result = await _reviewService.RejectReviewAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Review rejected" : "Failed to reject review",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/response")]
        public async Task<ActionResult<ApiResponseDto<bool>>> AddManagementResponse(Guid hotelId, Guid id, [FromBody] ManagementResponseDto responseDto)
        {
            var result = await _reviewService.AddManagementResponseAsync(id, responseDto.Response);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Response added successfully" : "Failed to add response",
                Data = result
            });
        }
    }

    public class ManagementResponseDto
    {
        public string Response { get; set; } = string.Empty;
    }
}
