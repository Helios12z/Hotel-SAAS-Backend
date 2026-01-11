using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IGuestProfileService
    {
        Task<GuestProfileDto?> GetProfileAsync(Guid userId);
        Task<GuestProfileDto?> UpdatePreferencesAsync(Guid userId, UpdateGuestPreferencesDto dto);
        Task<GuestBookingHistoryDto> GetBookingHistoryAsync(Guid userId, BookingHistoryFilterDto filter);
        Task<IEnumerable<RecentlyViewedHotelDto>> GetRecentlyViewedAsync(Guid userId, int limit = 10);
        Task TrackHotelViewAsync(Guid userId, Guid hotelId);
        Task ClearRecentlyViewedAsync(Guid userId);
    }
}
