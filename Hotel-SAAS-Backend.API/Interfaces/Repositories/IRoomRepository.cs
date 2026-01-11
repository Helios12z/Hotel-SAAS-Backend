using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid id);
        Task<Room?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Room>> GetByHotelAsync(Guid hotelId);
        Task<IEnumerable<Room>> GetByHotelAsync(Guid hotelId, RoomType? type, RoomStatus? status);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut, RoomType? type = null);
        Task<Room?> GetByRoomNumberAsync(Guid hotelId, string roomNumber);
        Task<Room> CreateAsync(Room room);
        Task<Room> UpdateAsync(Room room);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
    }
}
