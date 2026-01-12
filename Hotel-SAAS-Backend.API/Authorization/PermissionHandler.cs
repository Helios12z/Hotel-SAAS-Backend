using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Authorization
{
    public class PermissionHandler(
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var userIdClaim = httpContext.User.FindFirst("userId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                context.Fail();
                return;
            }

            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                context.Fail();
                return;
            }

            var permission = requirement.Permission;

            // SuperAdmin has system permissions (not hotel-specific)
            if (role == UserRole.SuperAdmin && !permission.StartsWith("hotel"))
            {
                context.Succeed(requirement);
                return;
            }

            // Get scope from token
            var brandIdClaim = httpContext.User.FindFirst("brandId")?.Value;
            var hotelIdClaim = httpContext.User.FindFirst("hotelId")?.Value;

            // Check hotel-scoped permission
            var hotelIdParam = httpContext.Request.RouteValues["hotelId"]?.ToString();
            if (hotelIdParam != null && Guid.TryParse(hotelIdParam, out var hotelId))
            {
                using var scope = _serviceProvider.CreateScope();
                var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

                // For SuperAdmin, only allow read operations on hotels
                if (role == UserRole.SuperAdmin)
                {
                    if (permission.EndsWith(".read"))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    context.Fail();
                    return;
                }

                // For BrandAdmin, check if hotel belongs to their brand
                if (role == UserRole.BrandAdmin && !string.IsNullOrEmpty(brandIdClaim))
                {
                    if (Guid.TryParse(brandIdClaim, out var brandId))
                    {
                        if (await permissionService.IsHotelInBrandAsync(hotelId, brandId))
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }

                // For HotelManager, check if hotel matches their assigned hotel
                if (role == UserRole.HotelManager && !string.IsNullOrEmpty(hotelIdClaim))
                {
                    if (Guid.TryParse(hotelIdClaim, out var assignedHotelId) && assignedHotelId == hotelId)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }

                // Check database for permission
                if (await permissionService.HasPermissionForHotelAsync(userId, hotelId, permission))
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            else
            {
                // Non-hotel scoped permission check
                using var scope = _serviceProvider.CreateScope();
                var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

                if (await permissionService.HasPermissionAsync(userId, permission))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }
}
