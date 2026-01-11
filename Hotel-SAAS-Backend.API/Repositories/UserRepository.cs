using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.Role == role && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByBrandAsync(Guid brandId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.BrandId == brandId && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByHotelAsync(Guid hotelId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.HotelId == hotelId && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> EmailVerifiedAsync(Guid userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null) return false;

            user.EmailVerified = DateTime.UtcNow.ToString();
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(u => u.Brand)
                .Include(u => u.Hotel)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }
    }
}
