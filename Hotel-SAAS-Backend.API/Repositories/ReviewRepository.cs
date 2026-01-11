using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class ReviewRepository(ApplicationDbContext context) : BaseRepository<Review>(context), IReviewRepository
    {
        public async Task<IEnumerable<Review>> GetByHotelAsync(Guid hotelId)
        {
            return await _dbSet
                .Include(r => r.Guest)
                .Include(r => r.Images)
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByGuestAsync(Guid guestId)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.Images)
                .AsNoTracking()
                .Where(r => r.GuestId == guestId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByStatusAsync(ReviewStatus status)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.Guest)
                .AsNoTracking()
                .Where(r => r.Status == status && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetVerifiedReviewsAsync(Guid hotelId)
        {
            return await _dbSet
                .Include(r => r.Guest)
                .Include(r => r.Images)
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId && r.IsVerified && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasReviewedAsync(Guid guestId, Guid bookingId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(r => r.GuestId == guestId && r.BookingId == bookingId && !r.IsDeleted);
        }

        public override async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.Guest)
                .Include(r => r.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }
    }
}
