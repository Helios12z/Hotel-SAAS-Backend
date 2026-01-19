using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IAmenityService
    {
        Task<AmenityDto?> GetAmenityByIdAsync(Guid id);
        Task<IEnumerable<AmenityDto>> GetAllAmenitiesAsync();
        Task<IEnumerable<AmenityDto>> GetAmenitiesByTypeAsync(AmenityType type);
        Task<IEnumerable<AmenityDto>> GetActiveAmenitiesAsync();
        Task<IEnumerable<AmenityDto>> GetActiveAmenitiesByTypeAsync(AmenityType type);
        Task<AmenityDto> CreateAmenityAsync(CreateAmenityDto createAmenityDto);
        Task<AmenityDto> UpdateAmenityAsync(Guid id, UpdateAmenityDto updateAmenityDto);
        Task<bool> DeleteAmenityAsync(Guid id);
    }
}
