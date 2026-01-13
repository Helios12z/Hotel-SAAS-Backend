using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Models.Constants;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly PermissionContext _permissionContext;

        public UsersController(IUserService userService, PermissionContext permissionContext)
        {
            _userService = userService;
            _permissionContext = permissionContext;
        }

        /// <summary>
        /// Create a new user with hierarchy-based permission check
        /// - SuperAdmin → can only create BrandAdmin
        /// - BrandAdmin → can only create HotelManager (must specify BrandId)
        /// - HotelManager → can only create Receptionist/Staff (must specify HotelId)
        /// - Receptionist/Staff → cannot create users
        /// </summary>
        [Authorize(Policy = "Permission:users.create")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(createUserDto.Email))
            {
                return BadRequest(new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = Messages.Auth.EmailRequired
                });
            }

            // SuperAdmin: can only create BrandAdmin
            if (_permissionContext.IsSuperAdmin)
            {
                if (createUserDto.Role != UserRole.BrandAdmin)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.SuperAdminOnlyBrandAdmin
                    });
                }

                if (!createUserDto.BrandId.HasValue)
                {
                    return BadRequest(new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.BrandIdRequiredForBrandAdmin
                    });
                }

                // Create BrandAdmin
                var brandAdminUser = await CreateUserInternal(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = brandAdminUser.Id }, new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = Messages.User.BrandAdminCreated,
                    Data = brandAdminUser
                });
            }

            // BrandAdmin: can only create HotelManager
            if (_permissionContext.IsBrandAdmin)
            {
                if (createUserDto.Role != UserRole.HotelManager)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.BrandAdminOnlyHotelManager
                    });
                }

                // Check if BrandId matches current user's brand
                if (!createUserDto.BrandId.HasValue)
                {
                    return BadRequest(new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.Subscription.BrandIdRequired
                    });
                }

                if (createUserDto.BrandId != _permissionContext.BrandId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.CannotCreateForDifferentBrand
                    });
                }

                // Require HotelId for HotelManager
                if (!createUserDto.HotelId.HasValue)
                {
                    return BadRequest(new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.HotelIdRequiredForHotelManager
                    });
                }

                // Create HotelManager
                var hotelManagerUser = await CreateUserInternal(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = hotelManagerUser.Id }, new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = Messages.User.HotelManagerCreated,
                    Data = hotelManagerUser
                });
            }

            // HotelManager: can only create Receptionist/Staff
            if (_permissionContext.IsHotelManager)
            {
                if (createUserDto.Role != UserRole.Receptionist && createUserDto.Role != UserRole.Staff)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.HotelManagerOnlyStaff
                    });
                }

                // Require HotelId
                if (!createUserDto.HotelId.HasValue)
                {
                    return BadRequest(new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.Hotel.HotelIdRequired
                    });
                }

                // Check if HotelId matches current user's hotel
                if (createUserDto.HotelId != _permissionContext.HotelId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.CannotCreateForDifferentHotel
                    });
                }

                // Create Receptionist/Staff
                var staffUser = await CreateUserInternal(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = staffUser.Id }, new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = $"{createUserDto.Role} created successfully",
                    Data = staffUser
                });
            }

            // Receptionist/Staff cannot create users
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<UserDto>
            {
                Success = false,
                Message = Messages.User.NoPermission
            });
        }

        private async Task<UserDto> CreateUserInternal(CreateUserDto dto)
        {
            // Create user entity - this is a simplified version
            // In production, use IAuthService.RegisterAsync or similar
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                BrandId = dto.BrandId,
                HotelId = dto.HotelId,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // TODO: Use actual user creation service
            // For now, return the DTO representation
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                BrandId = user.BrandId,
                HotelId = user.HotelId
            };
        }

        [Authorize(Policy = "Permission:users.read")]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetAllUsers()
        {
            var currentUser = _permissionContext.GetCurrentUser();

            // SuperAdmin: see all users
            if (_permissionContext.IsSuperAdmin)
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new ApiResponseDto<IEnumerable<UserDto>>
                {
                    Success = true,
                    Data = users
                });
            }

            // BrandAdmin: see users in their brand
            if (_permissionContext.IsBrandAdmin && _permissionContext.BrandId.HasValue)
            {
                var users = await _userService.GetUsersByBrandAsync(_permissionContext.BrandId.Value);
                return Ok(new ApiResponseDto<IEnumerable<UserDto>>
                {
                    Success = true,
                    Data = users
                });
            }

            // HotelManager: see users in their hotel
            if ((_permissionContext.IsHotelManager || _permissionContext.IsReceptionist) && _permissionContext.HotelId.HasValue)
            {
                var users = await _userService.GetUsersByHotelAsync(_permissionContext.HotelId.Value);
                return Ok(new ApiResponseDto<IEnumerable<UserDto>>
                {
                    Success = true,
                    Data = users
                });
            }

            return Forbid();
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
                    Message = Messages.User.NotFound
                });
            }

            return Ok(new ApiResponseDto<UserDto>
            {
                Success = true,
                Data = user
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:users.read")]
        public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUserById(Guid id)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

            // Allow users to view their own profile
            if (currentUserId == id)
            {
                var targetUser = await _userService.GetUserByIdAsync(id);
                if (targetUser == null)
                {
                    return NotFound(new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = Messages.User.NotFound
                    });
                }

                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Data = targetUser
                });
            }

            // Get user for scope-based access check
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = Messages.User.NotFound
                });
            }

            // SuperAdmin can access all users
            if (currentUserRole == UserRole.SuperAdmin)
            {
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Data = user
                });
            }

            // BrandAdmin can access users in their brand
            if (currentUserRole == UserRole.BrandAdmin &&
                user.BrandId == _permissionContext.BrandId)
            {
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Data = user
                });
            }

            // HotelManager can access users in their hotel
            if ((currentUserRole == UserRole.HotelManager || currentUserRole == UserRole.Receptionist) &&
                user.HotelId == _permissionContext.HotelId)
            {
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Data = user
                });
            }

            return Forbid();
        }

        [HttpGet("role/{role}")]
        [Authorize(Policy = "Permission:users.read")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetUsersByRole(UserRole role)
        {
            var currentUser = _permissionContext.GetCurrentUser();

            // Only SuperAdmin can list users by role
            if (!_permissionContext.IsSuperAdmin)
            {
                return Forbid();
            }

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
                    Message = Messages.User.UpdateFailed
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.User.ProfileUpdated,
                Data = true
            });
        }

        [Authorize(Policy = "Permission:users.update")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            var currentUser = _permissionContext.GetCurrentUser();

            // SuperAdmin can update any user
            if (_permissionContext.IsSuperAdmin)
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = Messages.User.UpdateSuccess,
                    Data = user
                });
            }

            // Check if user is in scope
            var targetUser = await _userService.GetUserByIdAsync(id);
            if (targetUser == null)
            {
                return NotFound(new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = Messages.User.NotFound
                });
            }

            // BrandAdmin can update users in their brand
            if (_permissionContext.IsBrandAdmin &&
                targetUser.BrandId == _permissionContext.BrandId)
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = Messages.User.UpdateSuccess,
                    Data = user
                });
            }

            // HotelManager can update users in their hotel
            if (_permissionContext.IsHotelManager &&
                targetUser.HotelId == _permissionContext.HotelId)
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                return Ok(new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = Messages.User.UpdateSuccess,
                    Data = user
                });
            }

            return Forbid();
        }

        [Authorize(Policy = "Permission:users.delete")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(Guid id)
        {
            var currentUser = _permissionContext.GetCurrentUser();

            // SuperAdmin can delete any user
            if (_permissionContext.IsSuperAdmin)
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? "User deleted successfully" : "Failed to delete user",
                    Data = result
                });
            }

            // Check if user is in scope
            var targetUser = await _userService.GetUserByIdAsync(id);
            if (targetUser == null)
            {
                return NotFound(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = Messages.User.NotFound
                });
            }

            // BrandAdmin can delete users in their brand
            if (_permissionContext.IsBrandAdmin &&
                targetUser.BrandId == _permissionContext.BrandId)
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? "User deleted successfully" : "Failed to delete user",
                    Data = result
                });
            }

            // HotelManager can delete users in their hotel
            if (_permissionContext.IsHotelManager &&
                targetUser.HotelId == _permissionContext.HotelId)
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Message = result ? "User deleted successfully" : "Failed to delete user",
                    Data = result
                });
            }

            return Forbid();
        }
    }
}
