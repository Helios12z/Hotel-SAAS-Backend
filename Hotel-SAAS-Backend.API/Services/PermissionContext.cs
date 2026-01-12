using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Services
{
    public class PermissionContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionService _permissionService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public PermissionContext(
            IHttpContextAccessor httpContextAccessor,
            IPermissionService permissionService,
            IUserRepository userRepository,
            ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
            _userRepository = userRepository;
            _context = context;
        }

        private Guid UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("userId");
                if (claim != null && Guid.TryParse(claim.Value, out var id))
                    return id;
                return Guid.Empty;
            }
        }

        public UserRole Role
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
                if (claim != null && Enum.TryParse<UserRole>(claim.Value, out var role))
                    return role;
                return UserRole.Guest;
            }
        }

        public Guid? BrandId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("brandId");
                if (claim != null && !string.IsNullOrEmpty(claim.Value) && Guid.TryParse(claim.Value, out var id))
                    return id;
                return null;
            }
        }

        public Guid? HotelId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("hotelId");
                if (claim != null && !string.IsNullOrEmpty(claim.Value) && Guid.TryParse(claim.Value, out var id))
                    return id;
                return null;
            }
        }

        public bool IsSuperAdmin => Role == UserRole.SuperAdmin;
        public bool IsBrandAdmin => Role == UserRole.BrandAdmin;
        public bool IsHotelManager => Role == UserRole.HotelManager;
        public bool IsReceptionist => Role == UserRole.Receptionist;
        public bool IsStaff => Role == UserRole.Staff;

        public async Task<bool> CanAccessAsync(string permission)
        {
            // SuperAdmin has system permissions (excluding hotel-specific)
            if (IsSuperAdmin && !permission.StartsWith("hotel"))
                return true;

            return await _permissionService.HasPermissionAsync(UserId, permission);
        }

        public async Task<bool> CanAccessHotelAsync(Guid hotelId, string permission)
        {
            // SuperAdmin can only view hotels, not modify
            if (IsSuperAdmin)
            {
                return permission.EndsWith(".read");
            }

            // BrandAdmin: access hotels within their brand
            if (IsBrandAdmin && BrandId.HasValue)
            {
                var hotel = await _context.Hotels.FindAsync(hotelId);
                if (hotel?.BrandId == BrandId.Value)
                    return true;
            }

            // HotelManager: access only their assigned hotel
            if (IsHotelManager && HotelId == hotelId)
                return true;

            // Receptionist/Staff: check permission for specific hotel
            return await _permissionService.HasPermissionForHotelAsync(UserId, hotelId, permission);
        }

        public async Task<bool> CanManageHotelsInBrandAsync(Guid brandId)
        {
            if (IsSuperAdmin) return true;
            if (IsBrandAdmin && BrandId == brandId) return true;
            return false;
        }

        public async Task<bool> CanCreateHotelsAsync()
        {
            // Only BrandAdmin can create hotels
            return IsBrandAdmin && BrandId.HasValue;
        }

        public async Task<bool> CanDeleteHotelsAsync()
        {
            // Only BrandAdmin can delete hotels
            return IsBrandAdmin && BrandId.HasValue;
        }

        public User GetCurrentUser()
        {
            if (UserId == Guid.Empty)
                return null!;

            return _userRepository.GetByIdAsync(UserId).Result!;
        }

        public bool CanAccessResource<T>(Guid resourceId, string permission) where T : class
        {
            // SuperAdmin can read all resources
            if (IsSuperAdmin && permission.EndsWith(".read"))
                return true;

            // For resources with BrandId or HotelId, check scope
            var entity = _context.Set<T>().Find(resourceId);
            if (entity == null) return false;

            // Check Brand scope
            var brandIdProp = typeof(T).GetProperty("BrandId");
            if (brandIdProp != null && brandIdProp.GetValue(entity) is Guid entityBrandId)
            {
                if (IsBrandAdmin && BrandId == entityBrandId)
                    return true;
            }

            // Check Hotel scope
            var hotelIdProp = typeof(T).GetProperty("HotelId");
            if (hotelIdProp != null && hotelIdProp.GetValue(entity) is Guid entityHotelId)
            {
                if (IsHotelManager && HotelId == entityHotelId)
                    return true;
                if ((IsReceptionist || IsStaff) && HotelId == entityHotelId)
                    return true;
            }

            return false;
        }
    }
}
