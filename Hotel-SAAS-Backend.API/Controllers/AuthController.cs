using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized(new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                return BadRequest(new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = "Email already exists"
                });
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Message = "Registration successful",
                Data = result
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            if (result == null)
            {
                return Unauthorized(new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = "Invalid refresh token"
                });
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.LogoutAsync(userId);

            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Logout successful" : "Logout failed",
                Data = result
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Failed to change password. Please check your current password."
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Password changed successfully",
                Data = true
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "If an account exists with this email, a password reset link will be sent.",
                Data = result
            });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (!result)
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Failed to reset password"
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Password reset successful",
                Data = true
            });
        }
    }
}
