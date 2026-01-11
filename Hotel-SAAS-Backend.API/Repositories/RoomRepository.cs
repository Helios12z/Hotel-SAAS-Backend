using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class RoomRepository(ApplicationDbContext context) : BaseRepository<Room>(context), IRoomRepository
    {
        public async Task<Room?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.Amenities)
                    .ThenInclude(ra => ra.Amenity)
                .Include(r => r.Images)
                .Include(r => r.ConnectingRoom)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<Room>> GetByHotelAsync(Guid hotelId)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId && !r.IsDeleted)
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetByHotelAsync(Guid hotelId, RoomType? type, RoomStatus? status)
        {
            var query = _dbSet
                .Include(r => r.Hotel)
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId && !r.IsDeleted);

            if (type.HasValue)
            {
                query = query.Where(r => r.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return await query
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut, RoomType? type = null)
        {
            var bookedRoomIds = await _context.BookingRooms
                .Where(br => br.Booking.HotelId == hotelId &&
                            br.Booking.Status.ToString() != "Cancelled" &&
                            br.Booking.Status.ToString() != "CheckedOut" &&
                            ((checkIn >= br.Booking.CheckInDate && checkIn < br.Booking.CheckOutDate) ||
                             (checkOut > br.Booking.CheckInDate && checkOut <= br.Booking.CheckOutDate) ||
                             (checkIn <= br.Booking.CheckInDate && checkOut >= br.Booking.CheckOutDate)))
                .Select(br => br.RoomId)
                .Distinct()
                .ToListAsync();

            var query = _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.Amenities)
                    .ThenInclude(ra => ra.Amenity)
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId &&
                            !r.IsDeleted &&
                            r.Status == RoomStatus.Available &&
                            !bookedRoomIds.Contains(r.Id));

            if (type.HasValue)
            {
                query = query.Where(r => r.Type == type.Value);
            }

            return await query
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<Room?> GetByRoomNumberAsync(Guid hotelId, string roomNumber)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.HotelId == hotelId && r.RoomNumber == roomNumber && !r.IsDeleted);
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var isBooked = await _context.BookingRooms
                .AnyAsync(br => br.RoomId == roomId &&
                              br.Booking.Status.ToString() != "Cancelled" &&
                              br.Booking.Status.ToString() != "CheckedOut" &&
                              ((checkIn >= br.Booking.CheckInDate && checkIn < br.Booking.CheckOutDate) ||
                               (checkOut > br.Booking.CheckInDate && checkOut <= br.Booking.CheckOutDate) ||
                               (checkIn <= br.Booking.CheckInDate && checkOut >= br.Booking.CheckOutDate)));

            return !isBooked;
        }

        public override async Task<Room?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }
    }
}
