using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetPaymentByIdAsync(Guid id);
        Task<IEnumerable<PaymentDto>> GetPaymentsByBookingAsync(Guid bookingId);
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
        Task<PaymentDto?> ProcessPaymentAsync(Guid id);
        Task<bool> RefundPaymentAsync(Guid id, decimal? amount, string? reason);
    }
}
