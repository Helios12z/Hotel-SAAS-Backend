using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services
{
    public class PermissionService(
        ApplicationDbContext context,
        IUserRepository userRepository) : IPermissionService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.AsNoTracking().ToListAsync();
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new List<string>();

            // SuperAdmin has all system permissions (excluding hotel-specific)
            if (user.Role == UserRole.SuperAdmin)
            {
                return await _context.Permissions
                    .Where(p => p.Resource != "hotel")
                    .Select(p => p.Name)
                    .ToListAsync();
            }

            // Get role permissions
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.Role == user.Role)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();

            return rolePermissions.Distinct().ToList();
        }

        public async Task<List<string>> GetUserPermissionsForHotelAsync(Guid userId, Guid hotelId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new List<string>();

            // SuperAdmin has all permissions for all hotels
            if (user.Role == UserRole.SuperAdmin)
            {
                return await _context.Permissions.Select(p => p.Name).ToListAsync();
            }

            var basePermissions = await GetUserPermissionsAsync(userId);

            // Get user hotel-specific permissions
            var hotelPermissions = await _context.UserHotelPermissions
                .Where(uhp => uhp.UserId == userId && uhp.HotelId == hotelId)
                .Select(uhp => uhp.Permission.Name)
                .ToListAsync();

            // Merge: base + hotel-specific
            return basePermissions.Union(hotelPermissions).Distinct().ToList();
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permission)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            return permissions.Contains(permission);
        }

        public async Task<bool> HasPermissionForHotelAsync(Guid userId, Guid hotelId, string permission)
        {
            var permissions = await GetUserPermissionsForHotelAsync(userId, hotelId);
            return permissions.Contains(permission);
        }

        public async Task GrantHotelPermissionAsync(Guid userId, Guid hotelId, string permission)
        {
            var permissionEntity = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permission);

            if (permissionEntity == null) return;

            var existing = await _context.UserHotelPermissions
                .FirstOrDefaultAsync(uhp =>
                    uhp.UserId == userId &&
                    uhp.HotelId == hotelId &&
                    uhp.PermissionId == permissionEntity.Id);

            if (existing != null)
            {
                existing.IsGranted = true;
            }
            else
            {
                _context.UserHotelPermissions.Add(new UserHotelPermission
                {
                    UserId = userId,
                    HotelId = hotelId,
                    PermissionId = permissionEntity.Id,
                    IsGranted = true
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RevokeHotelPermissionAsync(Guid userId, Guid hotelId, string permission)
        {
            var permissionEntity = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permission);

            if (permissionEntity == null) return;

            var existing = await _context.UserHotelPermissions
                .FirstOrDefaultAsync(uhp =>
                    uhp.UserId == userId &&
                    uhp.HotelId == hotelId &&
                    uhp.PermissionId == permissionEntity.Id);

            if (existing != null)
            {
                existing.IsGranted = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsHotelInBrandAsync(Guid hotelId, Guid brandId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            return hotel?.BrandId == brandId;
        }
    }
}
