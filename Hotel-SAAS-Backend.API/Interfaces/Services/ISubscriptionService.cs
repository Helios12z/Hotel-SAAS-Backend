using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<SubscriptionDto>> GetAllAsync();
        Task<SubscriptionDto?> GetActiveByBrandAsync(Guid brandId);
        Task<IEnumerable<SubscriptionDto>> GetByBrandAsync(Guid brandId);
        Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto);
        Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto dto);
        Task<SubscriptionDto> ChangePlanAsync(Guid id, ChangeSubscriptionPlanDto dto);
        Task<bool> CancelAsync(Guid id, CancelSubscriptionDto dto);
        Task<bool> RenewAsync(Guid id);
        
        // Invoice operations
        Task<IEnumerable<SubscriptionInvoiceDto>> GetInvoicesAsync(Guid subscriptionId);
        Task<SubscriptionInvoiceDto?> GetInvoiceByIdAsync(Guid invoiceId);
        Task<SubscriptionInvoiceDto> CreateInvoiceAsync(Guid subscriptionId);
        Task<bool> PayInvoiceAsync(Guid invoiceId, PayInvoiceDto dto);
        
        // Usage & Limits
        Task<bool> CanAddHotelAsync(Guid brandId);
        Task<bool> CanAddRoomAsync(Guid hotelId);
        Task<bool> CanAddUserAsync(Guid hotelId);
    }
}
