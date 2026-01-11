using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<BrandDto>>>> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(new ApiResponseDto<IEnumerable<BrandDto>>
            {
                Success = true,
                Data = brands
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<BrandDto>>> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound(new ApiResponseDto<BrandDto>
                {
                    Success = false,
                    Message = "Brand not found"
                });
            }

            return Ok(new ApiResponseDto<BrandDto>
            {
                Success = true,
                Data = brand
            });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<BrandDto>>> CreateBrand([FromBody] CreateBrandDto createBrandDto)
        {
            try
            {
                var brand = await _brandService.CreateBrandAsync(createBrandDto);
                return CreatedAtAction(nameof(GetBrandById), new { id = brand.Id }, new ApiResponseDto<BrandDto>
                {
                    Success = true,
                    Message = "Brand created successfully",
                    Data = brand
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<BrandDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<BrandDto>>> UpdateBrand(Guid id, [FromBody] UpdateBrandDto updateBrandDto)
        {
            try
            {
                var brand = await _brandService.UpdateBrandAsync(id, updateBrandDto);
                return Ok(new ApiResponseDto<BrandDto>
                {
                    Success = true,
                    Message = "Brand updated successfully",
                    Data = brand
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<BrandDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteBrand(Guid id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Brand deleted successfully" : "Failed to delete brand",
                Data = result
            });
        }
    }
}
