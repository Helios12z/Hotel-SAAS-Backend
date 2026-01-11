using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IRecentlyViewedRepository
    {
        Task<IEnumerable<RecentlyViewedHotel>> GetByUserIdAsync(Guid userId, int limit = 10);
        Task<RecentlyViewedHotel?> GetByUserAndHotelAsync(Guid userId, Guid hotelId);
        Task AddOrUpdateViewAsync(Guid userId, Guid hotelId);
        Task ClearUserHistoryAsync(Guid userId);
    }
}
