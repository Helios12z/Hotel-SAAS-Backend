using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdAsync(Guid id);
        Task<Subscription?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<Subscription?> GetActiveByBrandAsync(Guid brandId);
        Task<IEnumerable<Subscription>> GetByBrandAsync(Guid brandId);
        Task<IEnumerable<Subscription>> GetByStatusAsync(SubscriptionStatus status);
        Task<IEnumerable<Subscription>> GetExpiringAsync(int daysUntilExpiry);
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<Subscription> UpdateAsync(Subscription subscription);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasActiveSubscriptionAsync(Guid brandId);
    }
}
