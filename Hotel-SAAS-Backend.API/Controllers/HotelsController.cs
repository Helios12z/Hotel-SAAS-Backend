using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Services;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IRoomService _roomService;
        private readonly PermissionContext _permissionContext;

        public HotelsController(
            IHotelService hotelService,
            IRoomService roomService,
            PermissionContext permissionContext)
        {
            _hotelService = hotelService;
            _roomService = roomService;
            _permissionContext = permissionContext;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:hotels.read")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<HotelDto>>>> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(new ApiResponseDto<IEnumerable<HotelDto>>
            {
                Success = true,
                Data = hotels
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:hotels.read")]
        public async Task<ActionResult<ApiResponseDto<HotelDto>>> GetHotelById(Guid id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound(new ApiResponseDto<HotelDto>
                {
                    Success = false,
                    Message = "Hotel not found"
                });
            }

            return Ok(new ApiResponseDto<HotelDto>
            {
                Success = true,
                Data = hotel
            });
        }

        [HttpGet("{id}/details")]
        [Authorize(Policy = "Permission:hotels.read")]
        public async Task<ActionResult<ApiResponseDto<HotelDetailDto>>> GetHotelDetails(Guid id)
        {
            var hotel = await _hotelService.GetHotelDetailByIdAsync(id);
            if (hotel == null)
            {
                return NotFound(new ApiResponseDto<HotelDetailDto>
                {
                    Success = false,
                    Message = "Hotel not found"
                });
            }

            return Ok(new ApiResponseDto<HotelDetailDto>
            {
                Success = true,
                Data = hotel
            });
        }

        [HttpGet("brand/{brandId}")]
        [Authorize(Policy = "Permission:hotels.read")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<HotelDto>>>> GetHotelsByBrand(Guid brandId)
        {
            var hotels = await _hotelService.GetHotelsByBrandAsync(brandId);
            return Ok(new ApiResponseDto<IEnumerable<HotelDto>>
            {
                Success = true,
                Data = hotels
            });
        }

        [HttpGet("search")]
        [Authorize(Policy = "Permission:hotels.read")]
        public async Task<ActionResult<ApiResponseDto<PagedResultDto<HotelSearchResultDto>>>> SearchHotels(
            [FromQuery] HotelSearchRequestDto request)
        {
            var result = await _hotelService.SearchHotelsAdvancedAsync(request);
            return Ok(new ApiResponseDto<PagedResultDto<HotelSearchResultDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("{id}/availability")]
        [Authorize(Policy = "Permission:hotels.read")]
        public async Task<ActionResult<ApiResponseDto<HotelAvailabilityDto>>> CheckAvailability(
            Guid id,
            [FromQuery] DateTime checkInDate,
            [FromQuery] DateTime checkOutDate,
            [FromQuery] int numberOfGuests = 1)
        {
            if (checkInDate >= checkOutDate)
            {
                return BadRequest(new ApiResponseDto<HotelAvailabilityDto>
                {
                    Success = false,
                    Message = "Check-out date must be after check-in date"
                });
            }

            if (checkInDate < DateTime.UtcNow.Date)
            {
                return BadRequest(new ApiResponseDto<HotelAvailabilityDto>
                {
                    Success = false,
                    Message = "Check-in date cannot be in the past"
                });
            }

            var request = new RoomAvailabilityRequestDto
            {
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                NumberOfGuests = numberOfGuests
            };

            var result = await _roomService.CheckAvailabilityAsync(id, request);
            if (result == null)
            {
                return NotFound(new ApiResponseDto<HotelAvailabilityDto>
                {
                    Success = false,
                    Message = "Hotel not found"
                });
            }

            return Ok(new ApiResponseDto<HotelAvailabilityDto>
            {
                Success = true,
                Data = result
            });
        }

        // NOTE: SuperAdmin CANNOT create/update/delete hotels - only BrandAdmin can!
        // This is by design: SuperAdmin manages system, BrandAdmin manages hotels within their brand
        [Authorize(Policy = "Permission:hotels.create")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<HotelDto>>> CreateHotel([FromBody] CreateHotelDto createHotelDto)
        {
            // Double-check: only BrandAdmin can create hotels
            if (!_permissionContext.IsBrandAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<HotelDto>
                {
                    Success = false,
                    Message = "Only BrandAdmin can create hotels"
                });
            }

            // Set BrandId from current user's brand
            if (_permissionContext.BrandId.HasValue)
            {
                createHotelDto.BrandId = _permissionContext.BrandId.Value;
            }
            else
            {
                return BadRequest(new ApiResponseDto<HotelDto>
                {
                    Success = false,
                    Message = "Brand ID is required"
                });
            }

            try
            {
                var hotel = await _hotelService.CreateHotelAsync(createHotelDto);
                return CreatedAtAction(nameof(GetHotelById), new { id = hotel.Id }, new ApiResponseDto<HotelDto>
                {
                    Success = true,
                    Message = "Hotel created successfully",
                    Data = hotel
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Policy = "Permission:hotels.update")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<HotelDto>>> UpdateHotel(Guid id, [FromBody] UpdateHotelDto updateHotelDto)
        {
            // Check if user can access this hotel
            if (!await _permissionContext.CanAccessHotelAsync(id, Permissions.Hotels.Update))
            {
                return Forbid();
            }

            try
            {
                var hotel = await _hotelService.UpdateHotelAsync(id, updateHotelDto);
                return Ok(new ApiResponseDto<HotelDto>
                {
                    Success = true,
                    Message = "Hotel updated successfully",
                    Data = hotel
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<HotelDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Policy = "Permission:hotels.delete")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteHotel(Guid id)
        {
            // Only BrandAdmin can delete hotels
            if (!_permissionContext.IsBrandAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Only BrandAdmin can delete hotels"
                });
            }

            var result = await _hotelService.DeleteHotelAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Hotel deleted successfully" : "Failed to delete hotel",
                Data = result
            });
        }
    }
}
