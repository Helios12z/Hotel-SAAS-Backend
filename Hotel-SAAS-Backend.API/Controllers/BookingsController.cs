using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<BookingDetailDto>>> GetBookingById(Guid id)
        {
            var booking = await _bookingService.GetBookingDetailByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new ApiResponseDto<BookingDetailDto>
                {
                    Success = false,
                    Message = "Booking not found"
                });
            }

            return Ok(new ApiResponseDto<BookingDetailDto>
            {
                Success = true,
                Data = booking
            });
        }

        [HttpGet("confirmation/{confirmationNumber}")]
        public async Task<ActionResult<ApiResponseDto<BookingDto>>> GetByConfirmationNumber(string confirmationNumber)
        {
            var booking = await _bookingService.GetBookingByConfirmationNumberAsync(confirmationNumber);
            if (booking == null)
            {
                return NotFound(new ApiResponseDto<BookingDto>
                {
                    Success = false,
                    Message = "Booking not found"
                });
            }

            return Ok(new ApiResponseDto<BookingDto>
            {
                Success = true,
                Data = booking
            });
        }

        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<BookingDto>>>> GetMyBookings()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var bookings = await _bookingService.GetBookingsByGuestAsync(userId);
            return Ok(new ApiResponseDto<IEnumerable<BookingDto>>
            {
                Success = true,
                Data = bookings
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<BookingDto>>>> GetHotelBookings(Guid hotelId)
        {
            var bookings = await _bookingService.GetBookingsByHotelAsync(hotelId);
            return Ok(new ApiResponseDto<IEnumerable<BookingDto>>
            {
                Success = true,
                Data = bookings
            });
        }

        [Authorize]
        [HttpPost("calculate")]
        public async Task<ActionResult<ApiResponseDto<BookingCalculationDto>>> CalculatePrice([FromBody] CalculatePriceDto calculatePriceDto)
        {
            var calculation = await _bookingService.CalculatePriceAsync(calculatePriceDto);
            return Ok(new ApiResponseDto<BookingCalculationDto>
            {
                Success = true,
                Data = calculation
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<BookingDto>>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(createBookingDto);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, new ApiResponseDto<BookingDto>
                {
                    Success = true,
                    Message = "Booking created successfully",
                    Data = booking
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<BookingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<BookingDto>>> UpdateBooking(Guid id, [FromBody] UpdateBookingDto updateBookingDto)
        {
            try
            {
                var booking = await _bookingService.UpdateBookingAsync(id, updateBookingDto);
                return Ok(new ApiResponseDto<BookingDto>
                {
                    Success = true,
                    Message = "Booking updated successfully",
                    Data = booking
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<BookingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CancelBooking(Guid id, [FromBody] CancelBookingDto cancelDto)
        {
            var result = await _bookingService.CancelBookingAsync(id, cancelDto.Reason);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Booking cancelled successfully" : "Failed to cancel booking",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ConfirmBooking(Guid id)
        {
            var result = await _bookingService.ConfirmBookingAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Booking confirmed successfully" : "Failed to confirm booking",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("{id}/checkin")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CheckIn(Guid id)
        {
            var result = await _bookingService.CheckInAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Check-in successful" : "Failed to check in",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("{id}/checkout")]
        public async Task<ActionResult<ApiResponseDto<CheckOutResponseDto>>> CheckOut(Guid id, [FromBody] CheckOutRequestDto? request)
        {
            try
            {
                var result = await _bookingService.CheckOutAsync(id, request);
                return Ok(new ApiResponseDto<CheckOutResponseDto>
                {
                    Success = true,
                    Message = "Check-out successful",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<CheckOutResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // ============ Change Room ============

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("{id}/change-room")]
        public async Task<ActionResult<ApiResponseDto<BookingDto>>> ChangeRoom(Guid id, [FromBody] ChangeRoomRequestDto request)
        {
            try
            {
                var result = await _bookingService.ChangeRoomAsync(id, request);
                return Ok(new ApiResponseDto<BookingDto>
                {
                    Success = true,
                    Message = "Room changed successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<BookingDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // ============ Late Checkout ============

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("{id}/late-checkout/calculate")]
        public async Task<ActionResult<ApiResponseDto<LateCheckoutResponseDto>>> CalculateLateCheckout(Guid id, [FromBody] LateCheckoutRequestDto request)
        {
            try
            {
                var result = await _bookingService.CalculateLateCheckoutFeeAsync(id, request);
                return Ok(new ApiResponseDto<LateCheckoutResponseDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<LateCheckoutResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("{id}/late-checkout")]
        public async Task<ActionResult<ApiResponseDto<LateCheckoutResponseDto>>> ProcessLateCheckout(Guid id, [FromBody] LateCheckoutRequestDto request)
        {
            try
            {
                var result = await _bookingService.ProcessLateCheckoutAsync(id, request);
                return Ok(new ApiResponseDto<LateCheckoutResponseDto>
                {
                    Success = true,
                    Message = "Late checkout processed successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<LateCheckoutResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // ============ Additional Charges ============

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpGet("{id}/charges")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AdditionalChargeDto>>>> GetAdditionalCharges(Guid id)
        {
            var charges = await _bookingService.GetAdditionalChargesAsync(id);
            return Ok(new ApiResponseDto<IEnumerable<AdditionalChargeDto>>
            {
                Success = true,
                Data = charges
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpPost("charges")]
        public async Task<ActionResult<ApiResponseDto<AdditionalChargeDto>>> AddAdditionalCharge([FromBody] CreateAdditionalChargeDto request)
        {
            try
            {
                var result = await _bookingService.AddAdditionalChargeAsync(request);
                return Ok(new ApiResponseDto<AdditionalChargeDto>
                {
                    Success = true,
                    Message = "Additional charge added successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<AdditionalChargeDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager,Receptionist")]
        [HttpDelete("charges/{chargeId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> RemoveAdditionalCharge(Guid chargeId)
        {
            var result = await _bookingService.RemoveAdditionalChargeAsync(chargeId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Charge removed successfully" : "Failed to remove charge",
                Data = result
            });
        }
    }

    public class CancelBookingDto
    {
        public string? Reason { get; set; }
    }
}
