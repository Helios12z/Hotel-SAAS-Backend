using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface ISubscriptionInvoiceRepository
    {
        Task<SubscriptionInvoice?> GetByIdAsync(Guid id);
        Task<SubscriptionInvoice?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<IEnumerable<SubscriptionInvoice>> GetBySubscriptionAsync(Guid subscriptionId);
        Task<IEnumerable<SubscriptionInvoice>> GetByStatusAsync(InvoiceStatus status);
        Task<IEnumerable<SubscriptionInvoice>> GetOverdueAsync();
        Task<SubscriptionInvoice> CreateAsync(SubscriptionInvoice invoice);
        Task<SubscriptionInvoice> UpdateAsync(SubscriptionInvoice invoice);
        Task<string> GenerateInvoiceNumberAsync();
    }
}
