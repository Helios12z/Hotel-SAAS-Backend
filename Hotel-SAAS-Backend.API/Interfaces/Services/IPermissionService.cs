using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
        Task<List<string>> GetUserPermissionsForHotelAsync(Guid userId, Guid hotelId);
        Task<bool> HasPermissionAsync(Guid userId, string permission);
        Task<bool> HasPermissionForHotelAsync(Guid userId, Guid hotelId, string permission);
        Task GrantHotelPermissionAsync(Guid userId, Guid hotelId, string permission);
        Task RevokeHotelPermissionAsync(Guid userId, Guid hotelId, string permission);
        Task<bool> IsHotelInBrandAsync(Guid hotelId, Guid brandId);
    }
}
