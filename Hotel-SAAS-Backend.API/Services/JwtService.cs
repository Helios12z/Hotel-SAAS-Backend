using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hotel_SAAS_Backend.API.Services
{
    public class JwtService(IOptions<JwtOptions> jwtOptions)
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public string GenerateAccessToken(User user)
        {
            var permissions = GetUserPermissions(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("userId", user.Id.ToString()),
                new("permissions", string.Join(",", permissions)),
                new("scope", GetScope(user.Role))
            };

            if (user.BrandId.HasValue)
            {
                claims.Add(new Claim("BrandId", user.BrandId.Value.ToString()));
            }

            if (user.HotelId.HasValue)
            {
                claims.Add(new Claim("HotelId", user.HotelId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Base64Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private List<string> GetUserPermissions(User user)
        {
            return user.Role switch
            {
                UserRole.SuperAdmin => new List<string>
                {
                    Permissions.Brands.Create, Permissions.Brands.Read, Permissions.Brands.Update, Permissions.Brands.Delete,
                    Permissions.Subscriptions.Read, Permissions.Subscriptions.Update,
                    Permissions.Users.Create, Permissions.Users.Read, Permissions.Users.Update, Permissions.Users.Delete,
                    Permissions.Dashboard.ViewAll,
                    // SuperAdmin can only read hotels, not create/update/delete
                    Permissions.Hotels.Read
                },
                UserRole.BrandAdmin => new List<string>
                {
                    Permissions.Brands.Read,
                    Permissions.Hotels.Create, Permissions.Hotels.Read, Permissions.Hotels.Update, Permissions.Hotels.Delete,
                    Permissions.Users.Create, Permissions.Users.Read, Permissions.Users.Update, Permissions.Users.Delete,
                    Permissions.Dashboard.View
                },
                UserRole.HotelManager => new List<string>
                {
                    Permissions.Rooms.Create, Permissions.Rooms.Read, Permissions.Rooms.Update, Permissions.Rooms.Delete,
                    Permissions.Bookings.Create, Permissions.Bookings.Read, Permissions.Bookings.Update, Permissions.Bookings.Delete,
                    Permissions.Bookings.CheckIn, Permissions.Bookings.CheckOut,
                    Permissions.Dashboard.View,
                    Permissions.Hotels.Read
                },
                UserRole.Receptionist => new List<string>
                {
                    Permissions.Rooms.Read,
                    Permissions.Bookings.Read, Permissions.Bookings.CheckIn, Permissions.Bookings.CheckOut,
                    Permissions.Hotels.Read
                },
                UserRole.Staff => new List<string>
                {
                    Permissions.Rooms.Read,
                    Permissions.Bookings.Read,
                    Permissions.Hotels.Read
                },
                _ => new List<string> { Permissions.Hotels.Read }
            };
        }

        private string GetScope(UserRole role)
        {
            return role switch
            {
                UserRole.SuperAdmin => "system",
                UserRole.BrandAdmin => "brand",
                UserRole.HotelManager or UserRole.Receptionist or UserRole.Staff => "hotel",
                _ => "guest"
            };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Base64Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
