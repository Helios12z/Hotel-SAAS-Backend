using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services
{
    public class BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IPromotionService promotionService,
        IPromotionRepository promotionRepository,
        ApplicationDbContext context) : IBookingService
    {
        public async Task<BookingDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            return booking == null ? null : Mapper.ToDto(booking);
        }

        public async Task<BookingDto?> GetBookingByConfirmationNumberAsync(string confirmationNumber)
        {
            var booking = await bookingRepository.GetByConfirmationNumberAsync(confirmationNumber);
            return booking == null ? null : Mapper.ToDto(booking);
        }

        public async Task<BookingDetailDto?> GetBookingDetailByIdAsync(Guid id)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(id);
            return booking == null ? null : Mapper.ToDetailDto(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByGuestAsync(Guid guestId)
        {
            var bookings = await bookingRepository.GetByGuestAsync(guestId);
            return bookings.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByHotelAsync(Guid hotelId)
        {
            var bookings = await bookingRepository.GetByHotelAsync(hotelId);
            return bookings.Select(Mapper.ToDto);
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            // Generate confirmation number
            var confirmationNumber = "BK" + DateTime.UtcNow.Ticks.ToString().Substring(10);

            // Calculate pricing
            var calculation = await CalculatePriceInternalAsync(createBookingDto);

            decimal discountAmount = 0;
            string? appliedCouponCode = null;
            Guid? appliedPromotionId = null;

            // Apply coupon if provided
            if (!string.IsNullOrWhiteSpace(createBookingDto.CouponCode))
            {
                var validationDto = new ValidateCouponDto
                {
                    Code = createBookingDto.CouponCode,
                    HotelId = createBookingDto.HotelId,
                    BookingAmount = calculation.Subtotal,
                    CheckInDate = createBookingDto.CheckInDate,
                    CheckOutDate = createBookingDto.CheckOutDate
                };

                var validationResult = await promotionService.ValidateCouponAsync(validationDto, Guid.Empty);

                if (validationResult.IsValid && validationResult.CalculatedDiscount.HasValue)
                {
                    discountAmount = validationResult.CalculatedDiscount.Value;
                    appliedCouponCode = validationResult.Code;

                    // Get promotion and increment usage count
                    var promotion = await promotionRepository.GetByCodeAsync(createBookingDto.CouponCode);
                    if (promotion != null)
                    {
                        appliedPromotionId = promotion.Id;
                        await promotionRepository.IncrementUsageCountAsync(promotion.Id);
                    }
                }
            }

            var totalAmount = calculation.Subtotal + calculation.TaxAmount + calculation.ServiceFee - discountAmount;

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                HotelId = createBookingDto.HotelId,
                GuestId = Guid.Empty, // Will be set from context
                ConfirmationNumber = confirmationNumber,
                CheckInDate = createBookingDto.CheckInDate,
                CheckOutDate = createBookingDto.CheckOutDate,
                NumberOfGuests = createBookingDto.Rooms.Sum(r => r.NumberOfAdults + r.NumberOfChildren),
                NumberOfRooms = createBookingDto.Rooms.Count,
                Subtotal = calculation.Subtotal,
                TaxAmount = calculation.TaxAmount,
                ServiceFee = calculation.ServiceFee,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                Currency = createBookingDto.Currency ?? "USD",
                Status = BookingStatus.Pending,
                GuestName = createBookingDto.GuestName,
                GuestEmail = createBookingDto.GuestEmail,
                GuestPhoneNumber = createBookingDto.GuestPhoneNumber,
                SpecialRequests = createBookingDto.SpecialRequests,
                AppliedCouponCode = appliedCouponCode,
                AppliedPromotionId = appliedPromotionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create booking rooms
            foreach (var roomDto in createBookingDto.Rooms)
            {
                var room = await roomRepository.GetByIdAsync(roomDto.RoomId);
                if (room != null)
                {
                    booking.BookingRooms.Add(new BookingRoom
                    {
                        RoomId = roomDto.RoomId,
                        RoomNumber = room.RoomNumber,
                        Price = calculation.RoomPrices.First(rp => rp.RoomId == roomDto.RoomId).TotalPrice,
                        NumberOfAdults = roomDto.NumberOfAdults,
                        NumberOfChildren = roomDto.NumberOfChildren,
                        NumberOfInfants = roomDto.NumberOfInfants,
                        GuestName = roomDto.GuestName
                    });
                }
            }

            var createdBooking = await bookingRepository.CreateAsync(booking);
            return Mapper.ToDto(createdBooking);
        }

        public async Task<BookingDto> UpdateBookingAsync(Guid id, UpdateBookingDto updateBookingDto)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null) throw new Exception(Messages.Booking.NotFound);

            if (updateBookingDto.CheckInDate.HasValue) booking.CheckInDate = updateBookingDto.CheckInDate.Value;
            if (updateBookingDto.CheckOutDate.HasValue) booking.CheckOutDate = updateBookingDto.CheckOutDate.Value;
            if (updateBookingDto.NumberOfGuests.HasValue) booking.NumberOfGuests = updateBookingDto.NumberOfGuests.Value;
            if (updateBookingDto.SpecialRequests != null) booking.SpecialRequests = updateBookingDto.SpecialRequests;

            var updatedBooking = await bookingRepository.UpdateAsync(booking);
            return Mapper.ToDto(updatedBooking);
        }

        public async Task<bool> CancelBookingAsync(Guid id, string? reason)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = reason;

            await bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> ConfirmBookingAsync(Guid id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = BookingStatus.Confirmed;
            booking.ConfirmedAt = DateTime.UtcNow;

            await bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> CheckInAsync(Guid id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = BookingStatus.CheckedIn;
            booking.CheckedInAt = DateTime.UtcNow;

            // Update room status
            foreach (var bookingRoom in booking.BookingRooms)
            {
                var room = await roomRepository.GetByIdAsync(bookingRoom.RoomId);
                if (room != null)
                {
                    room.Status = RoomStatus.Occupied;
                    await roomRepository.UpdateAsync(room);
                }
            }

            await bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<CheckOutResponseDto> CheckOutAsync(Guid id, CheckOutRequestDto? request = null)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(id);
            if (booking == null) throw new Exception(Messages.Booking.NotFound);

            // Add additional charges from request
            var additionalChargesTotal = 0m;
            var charges = new List<AdditionalChargeDto>();

            if (request?.AdditionalCharges != null)
            {
                foreach (var chargeDto in request.AdditionalCharges)
                {
                    var charge = new AdditionalCharge
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        Type = chargeDto.Type,
                        Description = chargeDto.Description,
                        Amount = chargeDto.Amount,
                        Notes = chargeDto.Notes,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.AdditionalCharges.Add(charge);
                    additionalChargesTotal += chargeDto.Amount;
                }
            }

            // Update booking
            booking.Status = BookingStatus.CheckedOut;
            booking.CheckedOutAt = DateTime.UtcNow;
            booking.TotalAmount += additionalChargesTotal;

            // Update room status to cleaning
            foreach (var bookingRoom in booking.BookingRooms)
            {
                var room = await roomRepository.GetByIdAsync(bookingRoom.RoomId);
                if (room != null)
                {
                    room.Status = RoomStatus.Cleaning;
                    await roomRepository.UpdateAsync(room);
                }
            }

            await bookingRepository.UpdateAsync(booking);
            await context.SaveChangesAsync();

            // Get updated charges
            var updatedCharges = await context.AdditionalCharges
                .Where(c => c.BookingId == booking.Id)
                .ToListAsync();

            charges = updatedCharges.Select(c => new AdditionalChargeDto
            {
                Id = c.Id,
                Type = c.Type,
                Description = c.Description,
                Amount = c.Amount,
                IsPaid = c.IsPaid,
                PaidAt = c.PaidAt,
                PaymentMethod = c.PaymentMethod,
                CreatedAt = c.CreatedAt
            }).ToList();

            var amountPaid = booking.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            var balanceDue = booking.TotalAmount - amountPaid;

            return new CheckOutResponseDto
            {
                BookingId = booking.Id,
                Status = booking.Status,
                CheckedOutAt = booking.CheckedOutAt,
                RoomCharges = booking.TotalAmount - additionalChargesTotal,
                AdditionalCharges = additionalChargesTotal,
                TotalAmount = booking.TotalAmount,
                AmountPaid = amountPaid,
                BalanceDue = balanceDue > 0 ? balanceDue : 0,
                Charges = charges
            };
        }

        public async Task<BookingCalculationDto> CalculatePriceAsync(CalculatePriceDto calculatePriceDto)
        {
            return await CalculatePriceInternalAsync(new CreateBookingDto
            {
                HotelId = calculatePriceDto.HotelId,
                CheckInDate = calculatePriceDto.CheckInDate,
                CheckOutDate = calculatePriceDto.CheckOutDate,
                Rooms = calculatePriceDto.Rooms,
                Currency = calculatePriceDto.Currency
            });
        }

        // ============ Change Room ============

        public async Task<BookingDto> ChangeRoomAsync(Guid bookingId, ChangeRoomRequestDto request)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) throw new Exception(Messages.Booking.NotFound);

            if (booking.Status != BookingStatus.CheckedIn)
                throw new Exception(Messages.Booking.CannotChangeRoomStatus);

            var oldRoom = await roomRepository.GetByIdAsync(request.OldRoomId);
            if (oldRoom == null) throw new Exception(Messages.Booking.OldRoomNotFound);

            var newRoom = await roomRepository.GetByIdAsync(request.NewRoomId);
            if (newRoom == null) throw new Exception(Messages.Booking.NewRoomNotFound);

            if (newRoom.Status != RoomStatus.Available)
                throw new Exception(Messages.Booking.NewRoomNotAvailable);

            // Update booking room
            var bookingRoom = booking.BookingRooms.FirstOrDefault(br => br.RoomId == request.OldRoomId);
            if (bookingRoom == null) throw new Exception(Messages.Booking.BookingRoomNotFound);

            // Calculate price difference (simplified - using new room's base price)
            var numberOfNights = (int)(booking.CheckOutDate - booking.CheckInDate).TotalDays;
            var oldPrice = bookingRoom.Price;
            var newPrice = newRoom.BasePrice * numberOfNights;
            var priceDifference = newPrice - oldPrice;

            // Update room statuses
            oldRoom.Status = RoomStatus.Cleaning;
            newRoom.Status = RoomStatus.Occupied;

            bookingRoom.RoomId = request.NewRoomId;
            bookingRoom.RoomNumber = newRoom.RoomNumber;
            bookingRoom.Price = newPrice;
            booking.TotalAmount += priceDifference;

            await roomRepository.UpdateAsync(oldRoom);
            await roomRepository.UpdateAsync(newRoom);
            await bookingRepository.UpdateAsync(booking);

            return Mapper.ToDto(booking);
        }

        // ============ Late Checkout ============

        public async Task<LateCheckoutResponseDto> CalculateLateCheckoutFeeAsync(Guid bookingId, LateCheckoutRequestDto request)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) throw new Exception(Messages.Booking.NotFound);

            if (booking.Status != BookingStatus.CheckedIn)
                throw new Exception(Messages.Booking.CannotCalculateLateCheckOut);

            var originalCheckOut = booking.CheckOutDate;
            var newCheckOut = request.NewCheckOutTime;

            if (newCheckOut <= originalCheckOut)
                throw new Exception(Messages.Booking.NewCheckOutTimeInvalid);

            // Calculate extra hours (minimum 1 hour, maximum 24 hours)
            var extraHours = (int)Math.Ceiling((newCheckOut - originalCheckOut).TotalHours);
            extraHours = Math.Max(1, Math.Min(24, extraHours));

            // Calculate late fee (10% of daily rate per hour, max 50% of daily rate)
            var mainRoom = booking.BookingRooms.FirstOrDefault();
            if (mainRoom == null) throw new Exception(Messages.Booking.NoRoomsInBooking);

            var dailyRate = mainRoom.Price / (int)(booking.CheckOutDate - booking.CheckInDate).TotalDays;
            dailyRate = Math.Max(dailyRate, mainRoom.Room != null ? mainRoom.Room.BasePrice : dailyRate);

            var hourlyRate = dailyRate * 0.10m; // 10% per hour
            var maxLateFee = dailyRate * 0.50m; // Max 50% of daily rate
            var lateFeeAmount = Math.Min(hourlyRate * extraHours, maxLateFee);

            return new LateCheckoutResponseDto
            {
                LateFeeAmount = lateFeeAmount,
                ExtraHours = extraHours,
                HourlyRate = hourlyRate,
                OriginalCheckOut = originalCheckOut,
                NewCheckOut = newCheckOut,
                OriginalTotal = booking.TotalAmount,
                NewTotal = booking.TotalAmount + lateFeeAmount
            };
        }

        public async Task<LateCheckoutResponseDto> ProcessLateCheckoutAsync(Guid bookingId, LateCheckoutRequestDto request)
        {
            // First calculate the fee
            var feeCalculation = await CalculateLateCheckoutFeeAsync(bookingId, request);

            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) throw new Exception(Messages.Booking.NotFound);

            // Add late fee as additional charge
            var lateFeeCharge = new AdditionalCharge
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                Type = "LateCheckout",
                Description = $"Late checkout fee: {request.NewCheckOutTime:HH:mm} ({feeCalculation.ExtraHours} hours late)",
                Amount = feeCalculation.LateFeeAmount,
                Notes = request.Reason,
                CreatedAt = DateTime.UtcNow
            };
            context.AdditionalCharges.Add(lateFeeCharge);

            // Update booking check-out date
            booking.CheckOutDate = request.NewCheckOutTime;
            booking.TotalAmount += feeCalculation.LateFeeAmount;

            await bookingRepository.UpdateAsync(booking);
            await context.SaveChangesAsync();

            return feeCalculation;
        }

        // ============ Additional Charges ============

        public async Task<AdditionalChargeDto> AddAdditionalChargeAsync(CreateAdditionalChargeDto request)
        {
            var charge = new AdditionalCharge
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                Type = request.Type,
                Description = request.Description,
                Amount = request.Amount,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            context.AdditionalCharges.Add(charge);
            await context.SaveChangesAsync();

            return new AdditionalChargeDto
            {
                Id = charge.Id,
                Type = charge.Type,
                Description = charge.Description,
                Amount = charge.Amount,
                IsPaid = charge.IsPaid,
                PaidAt = charge.PaidAt,
                PaymentMethod = charge.PaymentMethod,
                CreatedAt = charge.CreatedAt
            };
        }

        public async Task<IEnumerable<AdditionalChargeDto>> GetAdditionalChargesAsync(Guid bookingId)
        {
            var charges = await context.AdditionalCharges
                .Where(c => c.BookingId == bookingId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return charges.Select(c => new AdditionalChargeDto
            {
                Id = c.Id,
                Type = c.Type,
                Description = c.Description,
                Amount = c.Amount,
                IsPaid = c.IsPaid,
                PaidAt = c.PaidAt,
                PaymentMethod = c.PaymentMethod,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<bool> RemoveAdditionalChargeAsync(Guid chargeId)
        {
            var charge = await context.AdditionalCharges.FindAsync(chargeId);
            if (charge == null) return false;

            context.AdditionalCharges.Remove(charge);
            await context.SaveChangesAsync();
            return true;
        }

        private async Task<BookingCalculationDto> CalculatePriceInternalAsync(CreateBookingDto bookingDto)
        {
            var numberOfNights = (int)(bookingDto.CheckOutDate - bookingDto.CheckInDate).TotalDays;
            var roomPrices = new List<BookingRoomPriceDto>();
            decimal subtotal = 0;

            foreach (var roomDto in bookingDto.Rooms)
            {
                var room = await roomRepository.GetByIdAsync(roomDto.RoomId);
                if (room != null)
                {
                    // Simple pricing logic - can be enhanced with date-based pricing
                    var pricePerNight = room.BasePrice;
                    var totalPrice = pricePerNight * numberOfNights;

                    roomPrices.Add(new BookingRoomPriceDto
                    {
                        RoomId = room.Id,
                        RoomNumber = room.RoomNumber,
                        RoomType = room.Type,
                        PricePerNight = pricePerNight,
                        TotalPrice = totalPrice,
                        NumberOfNights = numberOfNights
                    });

                    subtotal += totalPrice;
                }
            }

            var taxRate = 0.10m; // 10% tax
            var serviceFeeRate = 0.05m; // 5% service fee

            var taxAmount = subtotal * taxRate;
            var serviceFee = subtotal * serviceFeeRate;
            var totalAmount = subtotal + taxAmount + serviceFee;

            return new BookingCalculationDto
            {
                Subtotal = subtotal,
                TaxAmount = taxAmount,
                ServiceFee = serviceFee,
                DiscountAmount = 0,
                TotalAmount = totalAmount,
                Currency = bookingDto.Currency ?? "USD",
                NumberOfNights = numberOfNights,
                RoomPrices = roomPrices
            };
        }
    }
}
