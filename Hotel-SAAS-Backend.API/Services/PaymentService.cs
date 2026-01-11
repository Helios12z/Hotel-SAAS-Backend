using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Services
{
    public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
    {
        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await paymentRepository.GetByIdAsync(id);
            return payment == null ? null : Mapper.ToDto(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByBookingAsync(Guid bookingId)
        {
            var payments = await paymentRepository.GetByBookingAsync(bookingId);
            return payments.Select(Mapper.ToDto);
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            var payment = Mapper.ToEntity(createPaymentDto);
            var createdPayment = await paymentRepository.CreateAsync(payment);
            return Mapper.ToDto(createdPayment);
        }

        public async Task<PaymentDto?> ProcessPaymentAsync(Guid id)
        {
            var payment = await paymentRepository.GetByIdAsync(id);
            if (payment == null) return null;

            payment.Status = Models.Enums.PaymentStatus.Completed;
            payment.ProcessedAt = DateTime.UtcNow;

            var updatedPayment = await paymentRepository.UpdateAsync(payment);
            return Mapper.ToDto(updatedPayment);
        }

        public async Task<bool> RefundPaymentAsync(Guid id, decimal? amount, string? reason)
        {
            var payment = await paymentRepository.GetByIdAsync(id);
            if (payment == null) return false;

            payment.Status = Models.Enums.PaymentStatus.Refunded;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundAmount = amount ?? payment.Amount;
            payment.RefundReason = reason;

            await paymentRepository.UpdateAsync(payment);
            return true;
        }
    }
}
