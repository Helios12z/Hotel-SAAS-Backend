using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class BrandRepository(ApplicationDbContext context) : BaseRepository<Brand>(context), IBrandRepository
    {
        public override async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.Hotels)
                .Include(b => b.Subscriptions)
                    .ThenInclude(s => s.Plan)
                .AsNoTracking()
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }

        public override async Task<Brand?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.Hotels)
                .Include(b => b.Subscriptions)
                    .ThenInclude(s => s.Plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }
    }
}
