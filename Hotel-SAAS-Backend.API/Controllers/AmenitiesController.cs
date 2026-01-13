using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenityService _amenityService;

        public AmenitiesController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AmenityDto>>>> GetAllAmenities()
        {
            var amenities = await _amenityService.GetAllAmenitiesAsync();
            return Ok(new ApiResponseDto<IEnumerable<AmenityDto>>
            {
                Success = true,
                Data = amenities
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<AmenityDto>>> GetAmenityById(Guid id)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(id);
            if (amenity == null)
            {
                return NotFound(new ApiResponseDto<AmenityDto>
                {
                    Success = false,
                    Message = Messages.Hotel.AmenityNotFound
                });
            }

            return Ok(new ApiResponseDto<AmenityDto>
            {
                Success = true,
                Data = amenity
            });
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AmenityDto>>>> GetAmenitiesByType(AmenityType type)
        {
            var amenities = await _amenityService.GetAmenitiesByTypeAsync(type);
            return Ok(new ApiResponseDto<IEnumerable<AmenityDto>>
            {
                Success = true,
                Data = amenities
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<AmenityDto>>> CreateAmenity([FromBody] CreateAmenityDto createAmenityDto)
        {
            try
            {
                var amenity = await _amenityService.CreateAmenityAsync(createAmenityDto);
                return CreatedAtAction(nameof(GetAmenityById), new { id = amenity.Id }, new ApiResponseDto<AmenityDto>
                {
                    Success = true,
                    Message = Messages.Hotel.AmenityCreated,
                    Data = amenity
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<AmenityDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<AmenityDto>>> UpdateAmenity(Guid id, [FromBody] UpdateAmenityDto updateAmenityDto)
        {
            try
            {
                var amenity = await _amenityService.UpdateAmenityAsync(id, updateAmenityDto);
                return Ok(new ApiResponseDto<AmenityDto>
                {
                    Success = true,
                    Message = Messages.Hotel.AmenityUpdated,
                    Data = amenity
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<AmenityDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteAmenity(Guid id)
        {
            var result = await _amenityService.DeleteAmenityAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? Messages.Hotel.Deleted : Messages.Hotel.DeleteFailed,
                Data = result
            });
        }
    }
}
