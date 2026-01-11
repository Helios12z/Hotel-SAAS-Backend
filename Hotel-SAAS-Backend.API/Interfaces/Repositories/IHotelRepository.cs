using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IHotelRepository
    {
        Task<Hotel?> GetByIdAsync(Guid id);
        Task<Hotel?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<IEnumerable<Hotel>> GetByBrandAsync(Guid brandId);
        Task<IEnumerable<Hotel>> GetByCityAsync(string city);
        Task<IEnumerable<Hotel>> SearchAsync(string query, string? city = null, int? starRating = null);
        Task<Hotel> CreateAsync(Hotel hotel);
        Task<Hotel> UpdateAsync(Hotel hotel);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task UpdateRatingAsync(Guid hotelId, float newRating);
    }
}
