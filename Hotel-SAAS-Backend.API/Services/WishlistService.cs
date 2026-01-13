using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;

namespace Hotel_SAAS_Backend.API.Services
{
    public class WishlistService(
        IWishlistRepository wishlistRepository,
        IHotelRepository hotelRepository) : IWishlistService
    {
        public async Task<WishlistSummaryDto> GetUserWishlistAsync(Guid userId)
        {
            var wishlists = await wishlistRepository.GetByUserIdAsync(userId);
            var items = wishlists.Select(Mapper.ToDto).ToList();

            return new WishlistSummaryDto
            {
                TotalItems = items.Count,
                Items = items
            };
        }

        public async Task<WishlistDto?> GetByIdAsync(Guid id)
        {
            var wishlist = await wishlistRepository.GetByIdAsync(id);
            return wishlist == null ? null : Mapper.ToDto(wishlist);
        }

        public async Task<WishlistDto> AddToWishlistAsync(Guid userId, AddToWishlistDto dto)
        {
            // Check if hotel exists
            var hotel = await hotelRepository.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException(Messages.Hotel.NotFound);

            // Check if already in wishlist
            if (await wishlistRepository.ExistsAsync(userId, dto.HotelId))
                throw new InvalidOperationException(Messages.Misc.HotelAlreadyInWishlist);

            var wishlist = Mapper.ToEntity(dto, userId);
            var created = await wishlistRepository.CreateAsync(wishlist);

            // Reload with hotel data
            var result = await wishlistRepository.GetByUserAndHotelAsync(userId, dto.HotelId);
            return Mapper.ToDto(result!);
        }

        public async Task<WishlistDto?> UpdateNoteAsync(Guid userId, Guid hotelId, UpdateWishlistDto dto)
        {
            var wishlist = await wishlistRepository.GetByUserAndHotelAsync(userId, hotelId);
            if (wishlist == null) return null;

            // Need to get the tracked entity for update
            var trackedWishlist = await wishlistRepository.GetByIdAsync(wishlist.Id);
            if (trackedWishlist == null) return null;

            trackedWishlist.Note = dto.Note;
            trackedWishlist.UpdatedAt = DateTime.UtcNow;

            await wishlistRepository.UpdateAsync(trackedWishlist);

            var updated = await wishlistRepository.GetByUserAndHotelAsync(userId, hotelId);
            return Mapper.ToDto(updated!);
        }

        public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid hotelId)
        {
            var wishlist = await wishlistRepository.GetByUserAndHotelAsync(userId, hotelId);
            if (wishlist == null) return false;

            return await wishlistRepository.DeleteAsync(wishlist.Id);
        }

        public async Task<bool> IsInWishlistAsync(Guid userId, Guid hotelId)
        {
            return await wishlistRepository.ExistsAsync(userId, hotelId);
        }

        public async Task<bool> ToggleWishlistAsync(Guid userId, Guid hotelId)
        {
            var exists = await wishlistRepository.ExistsAsync(userId, hotelId);

            if (exists)
            {
                await RemoveFromWishlistAsync(userId, hotelId);
                return false; // Removed
            }
            else
            {
                await AddToWishlistAsync(userId, new AddToWishlistDto { HotelId = hotelId });
                return true; // Added
            }
        }
    }
}
