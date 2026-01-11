using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await userRepository.GetByIdAsync(id);
            return user == null ? null : Mapper.ToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await userRepository.GetAllAsync();
            return users.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await userRepository.GetByRoleAsync(role);
            return users.Select(Mapper.ToDto);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            if (updateUserDto.FirstName != null) user.FirstName = updateUserDto.FirstName;
            if (updateUserDto.LastName != null) user.LastName = updateUserDto.LastName;
            if (updateUserDto.PhoneNumber != null) user.PhoneNumber = updateUserDto.PhoneNumber;
            if (updateUserDto.Role.HasValue) user.Role = updateUserDto.Role.Value;
            if (updateUserDto.Status.HasValue) user.Status = updateUserDto.Status.Value;
            if (updateUserDto.BrandId.HasValue) user.BrandId = updateUserDto.BrandId.Value;
            if (updateUserDto.HotelId.HasValue) user.HotelId = updateUserDto.HotelId.Value;

            var updatedUser = await userRepository.UpdateAsync(user);
            return Mapper.ToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            await userRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateProfileAsync(Guid id, UpdateProfileDto updateProfileDto)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) return false;

            if (updateProfileDto.FirstName != null) user.FirstName = updateProfileDto.FirstName;
            if (updateProfileDto.LastName != null) user.LastName = updateProfileDto.LastName;
            if (updateProfileDto.PhoneNumber != null) user.PhoneNumber = updateProfileDto.PhoneNumber;
            if (updateProfileDto.AvatarUrl != null) user.AvatarUrl = updateProfileDto.AvatarUrl;
            if (updateProfileDto.Nationality != null) user.Nationality = updateProfileDto.Nationality;
            if (updateProfileDto.IdDocumentType != null) user.IdDocumentType = updateProfileDto.IdDocumentType;
            if (updateProfileDto.IdDocumentNumber != null) user.IdDocumentNumber = updateProfileDto.IdDocumentNumber;
            if (updateProfileDto.DateOfBirth.HasValue) user.DateOfBirth = updateProfileDto.DateOfBirth.Value;
            if (updateProfileDto.Address != null) user.Address = updateProfileDto.Address;
            if (updateProfileDto.City != null) user.City = updateProfileDto.City;
            if (updateProfileDto.Country != null) user.Country = updateProfileDto.Country;
            if (updateProfileDto.PostalCode != null) user.PostalCode = updateProfileDto.PostalCode;
            if (updateProfileDto.PreferredLanguage != null) user.PreferredLanguage = updateProfileDto.PreferredLanguage;
            if (updateProfileDto.PreferredCurrency != null) user.PreferredCurrency = updateProfileDto.PreferredCurrency;
            if (updateProfileDto.EmailNotificationsEnabled.HasValue) user.EmailNotificationsEnabled = updateProfileDto.EmailNotificationsEnabled.Value;
            if (updateProfileDto.SmsNotificationsEnabled.HasValue) user.SmsNotificationsEnabled = updateProfileDto.SmsNotificationsEnabled.Value;

            await userRepository.UpdateAsync(user);
            return true;
        }
    }
}
