using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class PaymentRepository(ApplicationDbContext context) : BaseRepository<Payment>(context), IPaymentRepository
    {
        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbSet
                .Include(p => p.Booking)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId && !p.IsDeleted);
        }

        public async Task<IEnumerable<Payment>> GetByBookingAsync(Guid bookingId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.BookingId == bookingId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .Include(p => p.Booking)
                .AsNoTracking()
                .Where(p => p.Status == status && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Booking)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }
    }
}
