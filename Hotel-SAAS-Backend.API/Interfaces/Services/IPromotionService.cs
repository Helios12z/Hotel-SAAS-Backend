using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IPromotionService
    {
        Task<PromotionDto?> GetByIdAsync(Guid id);
        Task<PromotionDto?> GetByCodeAsync(string code);
        Task<IEnumerable<PromotionDto>> GetAllAsync();
        Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null);
        Task<IEnumerable<PromotionDto>> GetPublicPromotionsAsync();
        Task<PromotionDto> CreateAsync(CreatePromotionDto dto);
        Task<PromotionDto> UpdateAsync(Guid id, UpdatePromotionDto dto);
        Task DeleteAsync(Guid id);
        Task<bool> ActivateAsync(Guid id);
        Task<bool> PauseAsync(Guid id);
        Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto dto, Guid userId);
        Task<decimal> CalculateDiscountAsync(string code, decimal bookingAmount, int numberOfNights);
    }
}
