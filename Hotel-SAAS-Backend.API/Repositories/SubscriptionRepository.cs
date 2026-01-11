using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class SubscriptionRepository(ApplicationDbContext context) 
        : BaseRepository<Subscription>(context), ISubscriptionRepository
    {
        public async Task<Subscription?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.Brand)
                .Include(s => s.Plan)
                .Include(s => s.Invoices.OrderByDescending(i => i.CreatedAt).Take(10))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<Subscription?> GetActiveByBrandAsync(Guid brandId)
        {
            return await _dbSet
                .Include(s => s.Plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => 
                    s.BrandId == brandId && 
                    !s.IsDeleted &&
                    (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial));
        }

        public async Task<IEnumerable<Subscription>> GetByBrandAsync(Guid brandId)
        {
            return await _dbSet
                .Include(s => s.Plan)
                .AsNoTracking()
                .Where(s => s.BrandId == brandId && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByStatusAsync(SubscriptionStatus status)
        {
            return await _dbSet
                .Include(s => s.Brand)
                .Include(s => s.Plan)
                .AsNoTracking()
                .Where(s => s.Status == status && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetExpiringAsync(int daysUntilExpiry)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
            return await _dbSet
                .Include(s => s.Brand)
                .Include(s => s.Plan)
                .AsNoTracking()
                .Where(s => 
                    s.EndDate <= expiryDate && 
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted)
                .OrderBy(s => s.EndDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(Guid brandId)
        {
            return await _dbSet.AnyAsync(s => 
                s.BrandId == brandId && 
                !s.IsDeleted &&
                (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial));
        }

        public override async Task<Subscription?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.Brand)
                .Include(s => s.Plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public override async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Brand)
                .Include(s => s.Plan)
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
