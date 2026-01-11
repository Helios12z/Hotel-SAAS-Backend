using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetByIdAsync(Guid id);
        Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId);
        Task<Wishlist?> GetByUserAndHotelAsync(Guid userId, Guid hotelId);
        Task<Wishlist> CreateAsync(Wishlist wishlist);
        Task<Wishlist> UpdateAsync(Wishlist wishlist);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid userId, Guid hotelId);
        Task<int> GetCountByUserAsync(Guid userId);
    }
}
