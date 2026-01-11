using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class NotificationRepository(ApplicationDbContext context) : BaseRepository<Notification>(context), INotificationRepository
    {
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsDeleted && n.ReadAt == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(n => n.UserId == userId && !n.IsDeleted && n.ReadAt == null);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null && notification.ReadAt == null)
            {
                notification.ReadAt = DateTime.UtcNow;
                notification.Status = NotificationStatus.Read;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            await _dbSet
                .Where(n => n.UserId == userId && n.ReadAt == null && !n.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.ReadAt, now)
                    .SetProperty(n => n.Status, NotificationStatus.Read)
                    .SetProperty(n => n.UpdatedAt, now));
        }

        public async Task MarkMultipleAsReadAsync(List<Guid> notificationIds)
        {
            var now = DateTime.UtcNow;
            await _dbSet
                .Where(n => notificationIds.Contains(n.Id) && n.ReadAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.ReadAt, now)
                    .SetProperty(n => n.Status, NotificationStatus.Read)
                    .SetProperty(n => n.UpdatedAt, now));
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
