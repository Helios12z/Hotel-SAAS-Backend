using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<DashboardSummaryDto>>> GetHotelDashboard(Guid hotelId)
        {
            var result = await _dashboardService.GetHotelDashboardAsync(hotelId);
            return Ok(new ApiResponseDto<DashboardSummaryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/revenue")]
        public async Task<ActionResult<ApiResponseDto<RevenueStatsDto>>> GetRevenueStats(Guid hotelId)
        {
            var result = await _dashboardService.GetRevenueStatsAsync(hotelId);
            return Ok(new ApiResponseDto<RevenueStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/bookings")]
        public async Task<ActionResult<ApiResponseDto<BookingStatsDto>>> GetBookingStats(Guid hotelId)
        {
            var result = await _dashboardService.GetBookingStatsAsync(hotelId);
            return Ok(new ApiResponseDto<BookingStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/occupancy")]
        public async Task<ActionResult<ApiResponseDto<OccupancyStatsDto>>> GetOccupancyStats(Guid hotelId)
        {
            var result = await _dashboardService.GetOccupancyStatsAsync(hotelId);
            return Ok(new ApiResponseDto<OccupancyStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/reviews")]
        public async Task<ActionResult<ApiResponseDto<ReviewStatsDto>>> GetReviewStats(Guid hotelId)
        {
            var result = await _dashboardService.GetReviewStatsAsync(hotelId);
            return Ok(new ApiResponseDto<ReviewStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/top-rooms")]
        public async Task<ActionResult<ApiResponseDto<List<TopRoomDto>>>> GetTopRooms(
            Guid hotelId,
            [FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetTopRoomsAsync(hotelId, count);
            return Ok(new ApiResponseDto<List<TopRoomDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/activities")]
        public async Task<ActionResult<ApiResponseDto<List<RecentActivityDto>>>> GetRecentActivities(
            Guid hotelId,
            [FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetRecentActivitiesAsync(hotelId, count);
            return Ok(new ApiResponseDto<List<RecentActivityDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/charts/revenue")]
        public async Task<ActionResult<ApiResponseDto<RevenueChartDto>>> GetRevenueChart(Guid hotelId)
        {
            var result = await _dashboardService.GetRevenueChartAsync(hotelId);
            return Ok(new ApiResponseDto<RevenueChartDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/charts/bookings")]
        public async Task<ActionResult<ApiResponseDto<BookingChartDto>>> GetBookingChart(Guid hotelId)
        {
            var result = await _dashboardService.GetBookingChartAsync(hotelId);
            return Ok(new ApiResponseDto<BookingChartDto>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<ApiResponseDto<DashboardSummaryDto>>> GetBrandDashboard(Guid brandId)
        {
            var result = await _dashboardService.GetBrandDashboardAsync(brandId);
            return Ok(new ApiResponseDto<DashboardSummaryDto>
            {
                Success = true,
                Data = result
            });
        }
    }
}
