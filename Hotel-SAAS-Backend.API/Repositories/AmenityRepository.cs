using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class AmenityRepository(ApplicationDbContext context) : BaseRepository<Amenity>(context), IAmenityRepository
    {
        public override async Task<IEnumerable<Amenity>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(a => !a.IsDeleted && a.IsActive)
                .OrderBy(a => a.Type)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Amenity>> GetByTypeAsync(AmenityType type)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(a => a.Type == type && !a.IsDeleted && a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
    }
}
