using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task<SubscriptionPlan?> GetByIdAsync(Guid id);
        Task<IEnumerable<SubscriptionPlan>> GetAllAsync();
        Task<IEnumerable<SubscriptionPlan>> GetActiveAsync();
        Task<SubscriptionPlan?> GetByTypeAsync(SubscriptionPlanType planType);
        Task<SubscriptionPlan> CreateAsync(SubscriptionPlan plan);
        Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan plan);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
