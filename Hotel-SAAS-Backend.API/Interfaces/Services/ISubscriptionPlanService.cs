using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<SubscriptionPlanDto>> GetAllAsync();
        Task<IEnumerable<SubscriptionPlanDto>> GetActiveAsync();
        Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanDto dto);
        Task<SubscriptionPlanDto> UpdateAsync(Guid id, UpdateSubscriptionPlanDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
    }
}
