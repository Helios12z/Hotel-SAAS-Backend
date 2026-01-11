using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IBrandService
    {
        Task<BrandDto?> GetBrandByIdAsync(Guid id);
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto);
        Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandDto updateBrandDto);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
