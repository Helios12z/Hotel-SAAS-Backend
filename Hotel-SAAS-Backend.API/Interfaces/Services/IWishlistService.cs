using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<WishlistSummaryDto> GetUserWishlistAsync(Guid userId);
        Task<WishlistDto?> GetByIdAsync(Guid id);
        Task<WishlistDto> AddToWishlistAsync(Guid userId, AddToWishlistDto dto);
        Task<WishlistDto?> UpdateNoteAsync(Guid userId, Guid hotelId, UpdateWishlistDto dto);
        Task<bool> RemoveFromWishlistAsync(Guid userId, Guid hotelId);
        Task<bool> IsInWishlistAsync(Guid userId, Guid hotelId);
        Task<bool> ToggleWishlistAsync(Guid userId, Guid hotelId);
    }
}
