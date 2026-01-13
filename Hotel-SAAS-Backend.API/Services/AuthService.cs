using BCrypt.Net;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Models.Constants;

namespace Hotel_SAAS_Backend.API.Services
{
    public class AuthService(
        IUserRepository userRepository,
        JwtService jwtService) : IAuthService
    {
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            if (user.Status != UserStatus.Active)
            {
                return null;
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(user);

            var accessToken = jwtService.GenerateAccessToken(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            // Store refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await userRepository.UpdateAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user exists
            if (await userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                return null;
            }

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                Role = registerDto.Role,
                BrandId = registerDto.BrandId,
                HotelId = registerDto.HotelId,
                Status = UserStatus.Active
            };

            user = await userRepository.CreateAsync(user);

            var accessToken = jwtService.GenerateAccessToken(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await userRepository.UpdateAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            // Find user with refresh token
            var users = await userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.RefreshToken == refreshTokenDto.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }

            var accessToken = jwtService.GenerateAccessToken(user);
            var newRefreshToken = jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await userRepository.UpdateAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            };
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                return false;
            }

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            // TODO: Implement email sending logic with reset token
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return false;
            }

            var user = await userRepository.GetByEmailAsync(resetPasswordDto.Email);
            if (user == null) return false;

            // TODO: Verify token
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            await userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> VerifyEmailAsync(Guid userId, string token)
        {
            // TODO: Implement email verification logic
            return await userRepository.EmailVerifiedAsync(userId);
        }
    }
}
