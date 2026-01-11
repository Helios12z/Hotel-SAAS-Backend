using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IAmenityRepository
    {
        Task<Amenity?> GetByIdAsync(Guid id);
        Task<IEnumerable<Amenity>> GetAllAsync();
        Task<IEnumerable<Amenity>> GetByTypeAsync(AmenityType type);
        Task<Amenity> CreateAsync(Amenity amenity);
        Task<Amenity> UpdateAsync(Amenity amenity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
