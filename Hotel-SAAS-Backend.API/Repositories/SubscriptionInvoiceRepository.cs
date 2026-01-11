using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class SubscriptionInvoiceRepository(ApplicationDbContext context) 
        : BaseRepository<SubscriptionInvoice>(context), ISubscriptionInvoiceRepository
    {
        public async Task<SubscriptionInvoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _dbSet
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted);
        }

        public async Task<IEnumerable<SubscriptionInvoice>> GetBySubscriptionAsync(Guid subscriptionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(i => i.SubscriptionId == subscriptionId && !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionInvoice>> GetByStatusAsync(InvoiceStatus status)
        {
            return await _dbSet
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.Brand)
                .AsNoTracking()
                .Where(i => i.Status == status && !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionInvoice>> GetOverdueAsync()
        {
            return await _dbSet
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.Brand)
                .AsNoTracking()
                .Where(i => 
                    i.Status == InvoiceStatus.Pending && 
                    i.DueDate < DateTime.UtcNow &&
                    !i.IsDeleted)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            var prefix = $"INV-{year}{month:D2}";
            
            var lastInvoice = await _dbSet
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            if (lastInvoice == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }

        public override async Task<SubscriptionInvoice?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }
    }
}
