using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class RoomService(IRoomRepository roomRepository) : IRoomService
    {
        public async Task<RoomDto?> GetRoomByIdAsync(Guid id)
        {
            var room = await roomRepository.GetByIdAsync(id);
            return room == null ? null : Mapper.ToDto(room);
        }

        public async Task<RoomDetailDto?> GetRoomDetailByIdAsync(Guid id)
        {
            var room = await roomRepository.GetByIdWithDetailsAsync(id);
            return room == null ? null : Mapper.ToDetailDto(room);
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(Guid hotelId)
        {
            var rooms = await roomRepository.GetByHotelAsync(hotelId);
            return rooms.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(Guid hotelId, RoomType? type, RoomStatus? status)
        {
            var rooms = await roomRepository.GetByHotelAsync(hotelId, type, status);
            return rooms.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut, RoomType? type = null)
        {
            var rooms = await roomRepository.GetAvailableRoomsAsync(hotelId, checkIn, checkOut, type);
            return rooms.Select(Mapper.ToDto);
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
        {
            var room = Mapper.ToEntity(createRoomDto);
            var createdRoom = await roomRepository.CreateAsync(room);
            return Mapper.ToDto(createdRoom);
        }

        public async Task<RoomDto> UpdateRoomAsync(Guid id, UpdateRoomDto updateRoomDto)
        {
            var room = await roomRepository.GetByIdAsync(id);
            if (room == null) throw new Exception("Room not found");

            Mapper.UpdateEntity(updateRoomDto, room);
            var updatedRoom = await roomRepository.UpdateAsync(room);
            return Mapper.ToDto(updatedRoom);
        }

        public async Task<bool> DeleteRoomAsync(Guid id)
        {
            await roomRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateRoomStatusAsync(Guid id, RoomStatus status)
        {
            var room = await roomRepository.GetByIdAsync(id);
            if (room == null) return false;

            room.Status = status;
            await roomRepository.UpdateAsync(room);
            return true;
        }
    }
}
