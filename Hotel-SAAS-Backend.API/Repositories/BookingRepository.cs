using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class BookingRepository(ApplicationDbContext context) : BaseRepository<Booking>(context), IBookingRepository
    {
        public async Task<Booking?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Include(b => b.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<Booking?> GetByConfirmationNumberAsync(string confirmationNumber)
        {
            return await _dbSet
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ConfirmationNumber == confirmationNumber && !b.IsDeleted);
        }

        public async Task<IEnumerable<Booking>> GetByGuestAsync(Guid guestId)
        {
            return await _dbSet
                .Include(b => b.Hotel)
                .Include(b => b.BookingRooms)
                .AsNoTracking()
                .Where(b => b.GuestId == guestId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByHotelAsync(Guid hotelId)
        {
            return await _dbSet
                .Include(b => b.Guest)
                .Include(b => b.BookingRooms)
                .AsNoTracking()
                .Where(b => b.HotelId == hotelId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status)
        {
            return await _dbSet
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .AsNoTracking()
                .Where(b => b.Status == status && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .AsNoTracking()
                .Where(b => !b.IsDeleted &&
                           ((b.CheckInDate >= startDate && b.CheckInDate <= endDate) ||
                            (b.CheckOutDate >= startDate && b.CheckOutDate <= endDate) ||
                            (b.CheckInDate <= startDate && b.CheckOutDate >= endDate)))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasActiveBookingAsync(Guid guestId, Guid hotelId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(b => b.GuestId == guestId &&
                              b.HotelId == hotelId &&
                              !b.IsDeleted &&
                              (b.Status == BookingStatus.Pending ||
                               b.Status == BookingStatus.Confirmed ||
                               b.Status == BookingStatus.CheckedIn));
        }

        public override async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<List<Guid>> GetBookedRoomIdsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut)
        {
            // L?y danh sách room IDs ?ã ???c book trong kho?ng th?i gian
            var bookedRoomIds = await _context.BookingRooms
                .Include(br => br.Booking)
                .Where(br =>
                    br.Booking.HotelId == hotelId &&
                    br.Booking.Status != BookingStatus.Cancelled &&
                    br.Booking.Status != BookingStatus.CheckedOut &&
                    // Check date overlap
                    br.Booking.CheckInDate < checkOut &&
                    br.Booking.CheckOutDate > checkIn)
                .Select(br => br.RoomId)
                .Distinct()
                .ToListAsync();

            return bookedRoomIds;
        }
    }
}
