using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IDashboardService
    {
        // For specific hotel
        Task<DashboardSummaryDto> GetHotelDashboardAsync(Guid hotelId);
        Task<RevenueStatsDto> GetRevenueStatsAsync(Guid hotelId);
        Task<BookingStatsDto> GetBookingStatsAsync(Guid hotelId);
        Task<OccupancyStatsDto> GetOccupancyStatsAsync(Guid hotelId);
        Task<ReviewStatsDto> GetReviewStatsAsync(Guid hotelId);
        Task<List<TopRoomDto>> GetTopRoomsAsync(Guid hotelId, int count = 5);
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync(Guid hotelId, int count = 10);
        Task<RevenueChartDto> GetRevenueChartAsync(Guid hotelId);
        Task<BookingChartDto> GetBookingChartAsync(Guid hotelId);
        
        // For brand (aggregate of all hotels)
        Task<DashboardSummaryDto> GetBrandDashboardAsync(Guid brandId);
    }
}
