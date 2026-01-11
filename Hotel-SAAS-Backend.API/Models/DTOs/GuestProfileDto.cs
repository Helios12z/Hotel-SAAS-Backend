using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class BookingHistoryFilterDto : PaginationRequestDto
    {
        public BookingStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? HotelId { get; set; }
    }

    public class UpdateGuestPreferencesDto
    {
        public string? PreferredLanguage { get; set; }
        public string? PreferredCurrency { get; set; }
        public bool? EmailNotificationsEnabled { get; set; }
        public bool? SmsNotificationsEnabled { get; set; }
        public string? Nationality { get; set; }
        public string? IdDocumentType { get; set; }
        public string? IdDocumentNumber { get; set; }
    }

    // Response DTOs
    public class GuestProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Nationality { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PreferredCurrency { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool SmsNotificationsEnabled { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Stats
        public int TotalBookings { get; set; }
        public int CompletedStays { get; set; }
        public int TotalReviews { get; set; }
    }

    public class RecentlyViewedHotelDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string? HotelImageUrl { get; set; }
        public string? City { get; set; }
        public int StarRating { get; set; }
        public float? AverageRating { get; set; }
        public decimal? MinPrice { get; set; }
        public DateTime ViewedAt { get; set; }
        public int ViewCount { get; set; }
    }

    public class GuestBookingHistoryDto
    {
        public PagedResultDto<BookingDto> Bookings { get; set; } = new();
        public BookingStatsSummaryDto Stats { get; set; } = new();
    }

    public class BookingStatsSummaryDto
    {
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public string? MostVisitedCity { get; set; }
    }
}
