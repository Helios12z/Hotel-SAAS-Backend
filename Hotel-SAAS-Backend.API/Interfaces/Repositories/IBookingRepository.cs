using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id);
        Task<Booking?> GetByIdWithDetailsAsync(Guid id);
        Task<Booking?> GetByConfirmationNumberAsync(string confirmationNumber);
        Task<IEnumerable<Booking>> GetByGuestAsync(Guid guestId);
        Task<IEnumerable<Booking>> GetByHotelAsync(Guid hotelId);
        Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status);
        Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasActiveBookingAsync(Guid guestId, Guid hotelId);
    }
}
