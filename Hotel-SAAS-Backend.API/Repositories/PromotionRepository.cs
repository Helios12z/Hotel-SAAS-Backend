using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class PromotionRepository(ApplicationDbContext context) : BaseRepository<Promotion>(context), IPromotionRepository
    {
        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null)
        {
            var now = DateTime.UtcNow;
            var query = _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .Where(p => !p.IsDeleted &&
                            p.Status == PromotionStatus.Active &&
                            p.StartDate <= now &&
                            p.EndDate >= now);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == null || p.BrandId == brandId);

            if (hotelId.HasValue)
                query = query.Where(p => p.HotelId == null || p.HotelId == hotelId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Promotion>> GetPublicPromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .Where(p => !p.IsDeleted &&
                            p.IsPublic &&
                            p.Status == PromotionStatus.Active &&
                            p.StartDate <= now &&
                            p.EndDate >= now)
                .OrderByDescending(p => p.DiscountValue)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _dbSet.AnyAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
        }

        public async Task IncrementUsageCountAsync(Guid promotionId)
        {
            var promotion = await _dbSet.FindAsync(promotionId);
            if (promotion != null)
            {
                promotion.CurrentUsageCount++;
                promotion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public override async Task<Promotion?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public override async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
