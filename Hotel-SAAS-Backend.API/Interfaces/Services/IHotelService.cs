using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IHotelService
    {
        Task<HotelDto?> GetHotelByIdAsync(Guid id);
        Task<HotelDetailDto?> GetHotelDetailByIdAsync(Guid id);
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
        Task<IEnumerable<HotelDto>> GetHotelsByBrandAsync(Guid brandId);
        Task<IEnumerable<HotelDto>> SearchHotelsAsync(string query, string? city = null, int? starRating = null);
        Task<HotelDto> CreateHotelAsync(CreateHotelDto createHotelDto);
        Task<HotelDto> UpdateHotelAsync(Guid id, UpdateHotelDto updateHotelDto);
        Task<bool> DeleteHotelAsync(Guid id);
        Task<bool> UpdateHotelImagesAsync(Guid id, List<HotelImageDto> images);
    }
}
