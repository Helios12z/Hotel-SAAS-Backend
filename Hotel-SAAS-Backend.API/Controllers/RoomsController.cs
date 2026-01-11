using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelId}/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<RoomDto>>>> GetRooms(Guid hotelId)
        {
            var rooms = await _roomService.GetRoomsByHotelAsync(hotelId);
            return Ok(new ApiResponseDto<IEnumerable<RoomDto>>
            {
                Success = true,
                Data = rooms
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<RoomDetailDto>>> GetRoomById(Guid hotelId, Guid id)
        {
            var room = await _roomService.GetRoomDetailByIdAsync(id);
            if (room == null)
            {
                return NotFound(new ApiResponseDto<RoomDetailDto>
                {
                    Success = false,
                    Message = "Room not found"
                });
            }

            return Ok(new ApiResponseDto<RoomDetailDto>
            {
                Success = true,
                Data = room
            });
        }

        [HttpGet("available")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<RoomDto>>>> GetAvailableRooms(
            Guid hotelId,
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            [FromQuery] RoomType? type = null)
        {
            var rooms = await _roomService.GetAvailableRoomsAsync(hotelId, checkIn, checkOut, type);
            return Ok(new ApiResponseDto<IEnumerable<RoomDto>>
            {
                Success = true,
                Data = rooms
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<RoomDto>>> CreateRoom(Guid hotelId, [FromBody] CreateRoomDto createRoomDto)
        {
            try
            {
                createRoomDto.HotelId = hotelId;
                var room = await _roomService.CreateRoomAsync(createRoomDto);
                return Ok(new ApiResponseDto<RoomDto>
                {
                    Success = true,
                    Message = "Room created successfully",
                    Data = room
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<RoomDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<RoomDto>>> UpdateRoom(Guid hotelId, Guid id, [FromBody] UpdateRoomDto updateRoomDto)
        {
            try
            {
                var room = await _roomService.UpdateRoomAsync(id, updateRoomDto);
                return Ok(new ApiResponseDto<RoomDto>
                {
                    Success = true,
                    Message = "Room updated successfully",
                    Data = room
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<RoomDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponseDto<bool>>> UpdateRoomStatus(Guid hotelId, Guid id, [FromBody] RoomStatusDto statusDto)
        {
            var result = await _roomService.UpdateRoomStatusAsync(id, statusDto.Status);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Room status updated" : "Failed to update room status",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteRoom(Guid hotelId, Guid id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Room deleted successfully" : "Failed to delete room",
                Data = result
            });
        }
    }

    public class RoomStatusDto
    {
        public RoomStatus Status { get; set; }
    }
}
