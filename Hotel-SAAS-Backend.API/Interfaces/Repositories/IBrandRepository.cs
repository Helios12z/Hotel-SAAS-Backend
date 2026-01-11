using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(Guid id);
        Task<IEnumerable<Brand>> GetAllAsync();
        Task<Brand> CreateAsync(Brand brand);
        Task<Brand> UpdateAsync(Brand brand);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
