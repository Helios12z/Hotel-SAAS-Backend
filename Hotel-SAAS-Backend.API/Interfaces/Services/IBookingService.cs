using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IBookingService
    {
        Task<BookingDto?> GetBookingByIdAsync(Guid id);
        Task<BookingDto?> GetBookingByConfirmationNumberAsync(string confirmationNumber);
        Task<BookingDetailDto?> GetBookingDetailByIdAsync(Guid id);
        Task<IEnumerable<BookingDto>> GetBookingsByGuestAsync(Guid guestId);
        Task<IEnumerable<BookingDto>> GetBookingsByHotelAsync(Guid hotelId);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingDto> UpdateBookingAsync(Guid id, UpdateBookingDto updateBookingDto);
        Task<bool> CancelBookingAsync(Guid id, string? reason);
        Task<bool> ConfirmBookingAsync(Guid id);
        Task<bool> CheckInAsync(Guid id);
        Task<bool> CheckOutAsync(Guid id);
        Task<BookingCalculationDto> CalculatePriceAsync(CalculatePriceDto calculatePriceDto);
    }
}
