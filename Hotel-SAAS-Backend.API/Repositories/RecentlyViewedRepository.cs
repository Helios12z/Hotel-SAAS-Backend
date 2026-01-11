using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class RecentlyViewedRepository(ApplicationDbContext context) : BaseRepository<RecentlyViewedHotel>(context), IRecentlyViewedRepository
    {
        public async Task<IEnumerable<RecentlyViewedHotel>> GetByUserIdAsync(Guid userId, int limit = 10)
        {
            return await _dbSet
                .Include(rv => rv.Hotel)
                    .ThenInclude(h => h.Brand)
                .Include(rv => rv.Hotel)
                    .ThenInclude(h => h.Rooms)
                .AsNoTracking()
                .Where(rv => rv.UserId == userId && !rv.IsDeleted && !rv.Hotel.IsDeleted)
                .OrderByDescending(rv => rv.ViewedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<RecentlyViewedHotel?> GetByUserAndHotelAsync(Guid userId, Guid hotelId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(rv => rv.UserId == userId && rv.HotelId == hotelId && !rv.IsDeleted);
        }

        public async Task AddOrUpdateViewAsync(Guid userId, Guid hotelId)
        {
            var existing = await GetByUserAndHotelAsync(userId, hotelId);

            if (existing != null)
            {
                existing.ViewedAt = DateTime.UtcNow;
                existing.ViewCount++;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            else
            {
                var newView = new RecentlyViewedHotel
                {
                    UserId = userId,
                    HotelId = hotelId,
                    ViewedAt = DateTime.UtcNow,
                    ViewCount = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _dbSet.AddAsync(newView);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearUserHistoryAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            await _dbSet
                .Where(rv => rv.UserId == userId && !rv.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rv => rv.IsDeleted, true)
                    .SetProperty(rv => rv.UpdatedAt, now));
        }
    }
}
