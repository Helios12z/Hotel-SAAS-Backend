using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IRoomService
    {
        Task<RoomDto?> GetRoomByIdAsync(Guid id);
        Task<RoomDetailDto?> GetRoomDetailByIdAsync(Guid id);
        Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(Guid hotelId);
        Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(Guid hotelId, RoomType? type, RoomStatus? status);
        Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut, RoomType? type = null);
        Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);
        Task<RoomDto> UpdateRoomAsync(Guid id, UpdateRoomDto updateRoomDto);
        Task<bool> DeleteRoomAsync(Guid id);
        Task<bool> UpdateRoomStatusAsync(Guid id, RoomStatus status);
        Task<HotelAvailabilityDto?> CheckAvailabilityAsync(Guid hotelId, RoomAvailabilityRequestDto request);

        // ============ Room Status & Maintenance ============
        Task<IEnumerable<RoomDto>> GetRoomsByStatusAsync(Guid hotelId, RoomStatus status);
        Task<bool> ReportRoomMaintenanceAsync(Guid roomId, RoomMaintenanceReportDto report);
        Task<IEnumerable<RoomDto>> GetMaintenanceRoomsAsync(Guid hotelId);
        Task<RoomDto> MarkRoomAvailableAsync(Guid roomId);
    }
}
