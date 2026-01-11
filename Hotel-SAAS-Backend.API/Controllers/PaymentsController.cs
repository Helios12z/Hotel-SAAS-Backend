using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/bookings/{bookingId}/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PaymentDto>>>> GetPayments(Guid bookingId)
        {
            var payments = await _paymentService.GetPaymentsByBookingAsync(bookingId);
            return Ok(new ApiResponseDto<IEnumerable<PaymentDto>>
            {
                Success = true,
                Data = payments
            });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<PaymentDto>>> GetPaymentById(Guid bookingId, Guid id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new ApiResponseDto<PaymentDto>
                {
                    Success = false,
                    Message = "Payment not found"
                });
            }

            return Ok(new ApiResponseDto<PaymentDto>
            {
                Success = true,
                Data = payment
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<PaymentDto>>> CreatePayment(Guid bookingId, [FromBody] CreatePaymentDto createPaymentDto)
        {
            try
            {
                createPaymentDto.BookingId = bookingId;
                var payment = await _paymentService.CreatePaymentAsync(createPaymentDto);
                return Ok(new ApiResponseDto<PaymentDto>
                {
                    Success = true,
                    Message = "Payment created successfully",
                    Data = payment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<PaymentDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("{id}/process")]
        public async Task<ActionResult<ApiResponseDto<PaymentDto>>> ProcessPayment(Guid bookingId, Guid id)
        {
            var payment = await _paymentService.ProcessPaymentAsync(id);
            if (payment == null)
            {
                return NotFound(new ApiResponseDto<PaymentDto>
                {
                    Success = false,
                    Message = "Payment not found"
                });
            }

            return Ok(new ApiResponseDto<PaymentDto>
            {
                Success = true,
                Message = "Payment processed successfully",
                Data = payment
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/refund")]
        public async Task<ActionResult<ApiResponseDto<bool>>> RefundPayment(Guid bookingId, Guid id, [FromBody] RefundDto refundDto)
        {
            var result = await _paymentService.RefundPaymentAsync(id, refundDto.Amount, refundDto.Reason);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Payment refunded successfully" : "Failed to refund payment",
                Data = result
            });
        }
    }

    public class RefundDto
    {
        public decimal? Amount { get; set; }
        public string? Reason { get; set; }
    }
}
