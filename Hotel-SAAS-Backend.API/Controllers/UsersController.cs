using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponseDto<IEnumerable<UserDto>>
            {
                Success = true,
                Data = users
            });
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDto<UserDto>>> GetMyProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponseDto<UserDto>
            {
                Success = true,
                Data = user
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUserById(Guid id)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

            // Only allow users to view their own profile or admins to view any profile
            if (currentUserId != id && currentUserRole != UserRole.SuperAdmin)
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponseDto<UserDto>
            {
                Success = true,
                Data = user
            });
        }

        [HttpGet("role/{role}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetUsersByRole(UserRole role)
        {
            var users = await _userService.GetUsersByRoleAsync(role);
            return Ok(new ApiResponseDto<IEnumerable<UserDto>>
            {
                Success = true,
                Data = users
            });
        }

        [HttpPatch("profile")]
        public async Task<ActionResult<ApiResponseDto<bool>>> UpdateMyProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userService.UpdateProfileAsync(userId, updateProfileDto);

            if (!result)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Failed to update profile"
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Profile updated successfully",
                Data = true
            });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "User deleted successfully" : "Failed to delete user",
                Data = result
            });
        }
    }
}
