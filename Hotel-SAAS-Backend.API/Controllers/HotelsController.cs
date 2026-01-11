using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
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
        public async Task<ActionResult<ApiResponseDto<IEnumerable<HotelDto>>>> SearchHotels(
            [FromQuery] string query,
            [FromQuery] string? city = null,
            [FromQuery] int? starRating = null)
        {
            var hotels = await _hotelService.SearchHotelsAsync(query, city, starRating);
            return Ok(new ApiResponseDto<IEnumerable<HotelDto>>
            {
                Success = true,
                Data = hotels
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<HotelDto>>> CreateHotel([FromBody] CreateHotelDto createHotelDto)
        {
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

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<HotelDto>>> UpdateHotel(Guid id, [FromBody] UpdateHotelDto updateHotelDto)
        {
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

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteHotel(Guid id)
        {
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
