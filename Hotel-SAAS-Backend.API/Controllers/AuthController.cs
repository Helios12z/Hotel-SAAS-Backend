using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
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
                    Message = Messages.Auth.InvalidCredentials
                });
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenCookie(result.RefreshToken);
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Message = Messages.Auth.LoginSuccess,
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
                    Message = Messages.Auth.EmailExists
                });
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenCookie(result.RefreshToken);
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Message = Messages.Auth.RegistrationSuccess,
                Data = result
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            // If RefreshToken is missing from body, try to get it from cookie
            if (string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
            {
                refreshTokenDto.RefreshToken = Request.Cookies["refreshToken"] ?? "";
            }

            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            if (result == null)
            {
                return Unauthorized(new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = Messages.Auth.InvalidRefreshToken
                });
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenCookie(result.RefreshToken);
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Message = Messages.Auth.RefreshTokenSuccess,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.LogoutAsync(userId);

            if (result)
            {
                Response.Cookies.Delete("refreshToken");
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? Messages.Auth.LogoutSuccess : Messages.Auth.LogoutFailed,
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
                    Message = Messages.Auth.PasswordChangeFailed
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.Auth.PasswordChangeSuccess,
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
                Message = Messages.Auth.ForgotPasswordSuccess,
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
                    Message = Messages.Auth.ResetPasswordFailed
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = Messages.Auth.ResetPasswordSuccess,
                Data = true
            });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(30)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
