using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class AmenityService(IAmenityRepository amenityRepository) : IAmenityService
    {
        public async Task<AmenityDto?> GetAmenityByIdAsync(Guid id)
        {
            var amenity = await amenityRepository.GetByIdAsync(id);
            return amenity == null ? null : Mapper.ToDto(amenity);
        }

        public async Task<IEnumerable<AmenityDto>> GetAllAmenitiesAsync()
        {
            var amenities = await amenityRepository.GetAllAsync();
            return amenities.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<AmenityDto>> GetAmenitiesByTypeAsync(AmenityType type)
        {
            var amenities = await amenityRepository.GetByTypeAsync(type);
            return amenities.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<AmenityDto>> GetActiveAmenitiesAsync()
        {
            var amenities = await amenityRepository.GetActiveAsync();
            return amenities.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<AmenityDto>> GetActiveAmenitiesByTypeAsync(AmenityType type)
        {
            var amenities = await amenityRepository.GetActiveByTypeAsync(type);
            return amenities.Select(Mapper.ToDto);
        }

        public async Task<AmenityDto> CreateAmenityAsync(CreateAmenityDto createAmenityDto)
        {
            var amenity = Mapper.ToEntity(createAmenityDto);
            var createdAmenity = await amenityRepository.CreateAsync(amenity);
            return Mapper.ToDto(createdAmenity);
        }

        public async Task<AmenityDto> UpdateAmenityAsync(Guid id, UpdateAmenityDto updateAmenityDto)
        {
            var amenity = await amenityRepository.GetByIdAsync(id);
            if (amenity == null) throw new Exception(Messages.Hotel.AmenityNotFound);

            Mapper.UpdateEntity(updateAmenityDto, amenity);
            var updatedAmenity = await amenityRepository.UpdateAsync(amenity);
            return Mapper.ToDto(updatedAmenity);
        }

        public async Task<bool> DeleteAmenityAsync(Guid id)
        {
            await amenityRepository.DeleteAsync(id);
            return true;
        }
    }
}
