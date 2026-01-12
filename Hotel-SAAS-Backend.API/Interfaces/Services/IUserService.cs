using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<UserDto>> GetUsersByBrandAsync(Guid brandId);
        Task<IEnumerable<UserDto>> GetUsersByHotelAsync(Guid hotelId);
        Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> UpdateProfileAsync(Guid id, UpdateProfileDto updateProfileDto);
    }
}
