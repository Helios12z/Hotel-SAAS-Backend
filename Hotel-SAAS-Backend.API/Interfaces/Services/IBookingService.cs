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
        Task<CheckOutResponseDto> CheckOutAsync(Guid id, CheckOutRequestDto? request = null);
        Task<BookingCalculationDto> CalculatePriceAsync(CalculatePriceDto calculatePriceDto);

        // ============ Change Room ============
        Task<BookingDto> ChangeRoomAsync(Guid bookingId, ChangeRoomRequestDto request);

        // ============ Late Checkout ============
        Task<LateCheckoutResponseDto> CalculateLateCheckoutFeeAsync(Guid bookingId, LateCheckoutRequestDto request);
        Task<LateCheckoutResponseDto> ProcessLateCheckoutAsync(Guid bookingId, LateCheckoutRequestDto request);

        // ============ Additional Charges ============
        Task<AdditionalChargeDto> AddAdditionalChargeAsync(CreateAdditionalChargeDto request);
        Task<IEnumerable<AdditionalChargeDto>> GetAdditionalChargesAsync(Guid bookingId);
        Task<bool> RemoveAdditionalChargeAsync(Guid chargeId);
    }
}
