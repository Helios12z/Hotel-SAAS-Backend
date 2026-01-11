using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateBookingDto
    {
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<BookingRoomDto> Rooms { get; set; } = new();
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhoneNumber { get; set; }
        public string? GuestAddress { get; set; }
        public string? GuestNationality { get; set; }
        public string? SpecialRequests { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Currency { get; set; } = "USD";
    }

    public class BookingRoomDto
    {
        public Guid RoomId { get; set; }
        public int NumberOfAdults { get; set; } = 1;
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfInfants { get; set; } = 0;
        public string? GuestName { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class UpdateBookingDto
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class CalculatePriceDto
    {
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<BookingRoomDto> Rooms { get; set; } = new();
        public string? Currency { get; set; } = "USD";
    }

    // Response DTOs
    public class BookingDto : BaseDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public Guid GuestId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string ConfirmationNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public BookingStatus Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? BookedAt { get; set; }
    }

    public class BookingDetailDto : BookingDto
    {
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? SpecialRequests { get; set; }
        public string? CancellationPolicy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public List<BookingRoomDetailDto> Rooms { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
        public string? HotelImageUrl { get; set; }
        public string? HotelAddress { get; set; }
        public string? HotelCity { get; set; }
        public string? HotelPhoneNumber { get; set; }
    }

    public class BookingRoomDetailDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public decimal Price { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public string? GuestName { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class BookingCalculationDto
    {
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public int NumberOfNights { get; set; }
        public List<BookingRoomPriceDto> RoomPrices { get; set; } = new();
    }

    public class BookingRoomPriceDto
    {
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public RoomType RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfNights { get; set; }
    }
}
