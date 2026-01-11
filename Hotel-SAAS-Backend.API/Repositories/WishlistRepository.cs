using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class WishlistRepository(ApplicationDbContext context) : BaseRepository<Wishlist>(context), IWishlistRepository
    {
        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Brand)
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Rooms)
                .AsNoTracking()
                .Where(w => w.UserId == userId && !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetByUserAndHotelAsync(Guid userId, Guid hotelId)
        {
            return await _dbSet
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.HotelId == hotelId && !w.IsDeleted);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid hotelId)
        {
            return await _dbSet.AnyAsync(w => w.UserId == userId && w.HotelId == hotelId && !w.IsDeleted);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _dbSet.CountAsync(w => w.UserId == userId && !w.IsDeleted);
        }

        public override async Task<Wishlist?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);
        }

        public new async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
