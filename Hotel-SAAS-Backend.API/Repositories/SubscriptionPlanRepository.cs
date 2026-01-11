using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class SubscriptionPlanRepository(ApplicationDbContext context) 
        : BaseRepository<SubscriptionPlan>(context), ISubscriptionPlanRepository
    {
        public async Task<IEnumerable<SubscriptionPlan>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.MonthlyPrice)
                .ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByTypeAsync(SubscriptionPlanType planType)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlanType == planType && p.IsActive && !p.IsDeleted);
        }

        public override async Task<SubscriptionPlan?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public override async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.MonthlyPrice)
                .ToListAsync();
        }
    }
}
