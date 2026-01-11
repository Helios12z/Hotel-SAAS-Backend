using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IPromotionService promotionService,
        IPromotionRepository promotionRepository) : IBookingService
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
            if (booking == null) throw new Exception("Booking not found");

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

        public async Task<bool> CheckOutAsync(Guid id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.Status = BookingStatus.CheckedOut;
            booking.CheckedOutAt = DateTime.UtcNow;

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
            return true;
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
