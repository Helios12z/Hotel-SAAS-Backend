using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Services
{
    public class HotelService(IHotelRepository hotelRepository) : IHotelService
    {
        public async Task<HotelDto?> GetHotelByIdAsync(Guid id)
        {
            var hotel = await hotelRepository.GetByIdAsync(id);
            return hotel == null ? null : Mapper.ToDto(hotel);
        }

        public async Task<HotelDetailDto?> GetHotelDetailByIdAsync(Guid id)
        {
            var hotel = await hotelRepository.GetByIdWithDetailsAsync(id);
            return hotel == null ? null : Mapper.ToDetailDto(hotel);
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await hotelRepository.GetAllAsync();
            return hotels.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<HotelDto>> GetHotelsByBrandAsync(Guid brandId)
        {
            var hotels = await hotelRepository.GetByBrandAsync(brandId);
            return hotels.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<HotelDto>> SearchHotelsAsync(string query, string? city = null, int? starRating = null)
        {
            var hotels = await hotelRepository.SearchAsync(query, city, starRating);
            return hotels.Select(Mapper.ToDto);
        }

        public async Task<HotelDto> CreateHotelAsync(CreateHotelDto createHotelDto)
        {
            var hotel = Mapper.ToEntity(createHotelDto);
            var createdHotel = await hotelRepository.CreateAsync(hotel);
            return Mapper.ToDto(createdHotel);
        }

        public async Task<HotelDto> UpdateHotelAsync(Guid id, UpdateHotelDto updateHotelDto)
        {
            var hotel = await hotelRepository.GetByIdAsync(id);
            if (hotel == null) throw new Exception("Hotel not found");

            Mapper.UpdateEntity(updateHotelDto, hotel);
            var updatedHotel = await hotelRepository.UpdateAsync(hotel);
            return Mapper.ToDto(updatedHotel);
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            await hotelRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateHotelImagesAsync(Guid id, List<HotelImageDto> images)
        {
            // TODO: Implement image update logic
            return true;
        }
    }
}
