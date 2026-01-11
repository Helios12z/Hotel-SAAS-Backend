using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IPromotionRepository
    {
        Task<Promotion?> GetByIdAsync(Guid id);
        Task<Promotion?> GetByCodeAsync(string code);
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null);
        Task<IEnumerable<Promotion>> GetPublicPromotionsAsync();
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion> UpdateAsync(Promotion promotion);
        Task DeleteAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task IncrementUsageCountAsync(Guid promotionId);
    }
}
