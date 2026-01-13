using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Services
{
    public class BrandService(IBrandRepository brandRepository) : IBrandService
    {
        public async Task<BrandDto?> GetBrandByIdAsync(Guid id)
        {
            var brand = await brandRepository.GetByIdAsync(id);
            return brand == null ? null : Mapper.ToDto(brand);
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await brandRepository.GetAllAsync();
            return brands.Select(Mapper.ToDto);
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var brand = Mapper.ToEntity(createBrandDto);
            var createdBrand = await brandRepository.CreateAsync(brand);
            return Mapper.ToDto(createdBrand);
        }

        public async Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandDto updateBrandDto)
        {
            var brand = await brandRepository.GetByIdAsync(id);
            if (brand == null) throw new Exception(Messages.Subscription.BrandNotFound);

            Mapper.UpdateEntity(updateBrandDto, brand);
            var updatedBrand = await brandRepository.UpdateAsync(brand);
            return Mapper.ToDto(updatedBrand);
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            await brandRepository.DeleteAsync(id);
            return true;
        }
    }
}
