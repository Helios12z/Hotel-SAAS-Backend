namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateHotelDto
    {
        public Guid BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public int StarRating { get; set; } = 3;
        public string? TaxId { get; set; }

        // Time Config
        public string? CheckInTime { get; set; } = "14:00";
        public string? CheckOutTime { get; set; } = "12:00";

        // Guest Config
        public int MaxAdultsPerRoom { get; set; } = 2;
        public int MaxChildrenPerRoom { get; set; } = 1;
        public int MaxGuestsPerRoom { get; set; } = 4;
        public bool AllowExtraBed { get; set; } = false;
        public decimal? ExtraBedPrice { get; set; }

        // Booking Rules
        public int MinNights { get; set; } = 1;
        public int MaxNights { get; set; } = 30;
        public int MinAdvanceBookingHours { get; set; } = 24;
        public int MaxAdvanceBookingDays { get; set; } = 365;

        // Payment Config
        public bool EnableStripePayment { get; set; } = true;
        public bool EnablePayAtHotel { get; set; } = true;
        public string? StripeAccountId { get; set; }

        // Tax & Fee Config
        public decimal TaxRate { get; set; } = 0.10m;
        public decimal ServiceFeeRate { get; set; } = 0.05m;

        // Policies
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public string? SmokingPolicy { get; set; }

        // Hotel Info
        public int? TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        public decimal? CommissionRate { get; set; }
    }

    public class UpdateHotelDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public int? StarRating { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsVerified { get; set; }

        // Time Config
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }

        // Guest Config
        public int? MaxAdultsPerRoom { get; set; }
        public int? MaxChildrenPerRoom { get; set; }
        public int? MaxGuestsPerRoom { get; set; }
        public bool? AllowExtraBed { get; set; }
        public decimal? ExtraBedPrice { get; set; }

        // Booking Rules
        public int? MinNights { get; set; }
        public int? MaxNights { get; set; }
        public int? MinAdvanceBookingHours { get; set; }
        public int? MaxAdvanceBookingDays { get; set; }

        // Payment Config
        public bool? EnableStripePayment { get; set; }
        public bool? EnablePayAtHotel { get; set; }
        public string? StripeAccountId { get; set; }

        // Tax & Fee Config
        public decimal? TaxRate { get; set; }
        public decimal? ServiceFeeRate { get; set; }

        // Policies
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public string? SmokingPolicy { get; set; }

        // Hotel Info
        public int? TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        public decimal? CommissionRate { get; set; }
    }

    // Hotel Settings DTO - Config riêng cho từng hotel (cho BrandAdmin edit)
    public class HotelSettingsDto
    {
        // Time Config
        public string CheckInTime { get; set; } = "14:00";
        public string CheckOutTime { get; set; } = "12:00";

        // Guest Config
        public int MaxAdultsPerRoom { get; set; } = 2;
        public int MaxChildrenPerRoom { get; set; } = 1;
        public int MaxGuestsPerRoom { get; set; } = 4;
        public bool AllowExtraBed { get; set; } = false;
        public decimal? ExtraBedPrice { get; set; }

        // Booking Rules
        public int MinNights { get; set; } = 1;
        public int MaxNights { get; set; } = 30;
        public int MinAdvanceBookingHours { get; set; } = 24;
        public int MaxAdvanceBookingDays { get; set; } = 365;

        // Payment Config
        public bool EnableStripePayment { get; set; } = true;
        public bool EnablePayAtHotel { get; set; } = true;
        public string? StripeAccountId { get; set; }

        // Tax & Fee Config
        public decimal TaxRate { get; set; } = 0.10m;
        public decimal ServiceFeeRate { get; set; } = 0.05m;

        // Policies
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public string? SmokingPolicy { get; set; }
    }

    public class HotelImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
        public string? Category { get; set; }
    }

    // Response DTOs
    public class HotelDto : BaseDto
    {
        public Guid BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int StarRating { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public float? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public decimal? MinPrice { get; set; }
    }

    public class HotelDetailDto : HotelDto
    {
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public int? TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        public string? TaxId { get; set; }
        public string? SmokingPolicy { get; set; }

        // Settings - Các config riêng của hotel
        public HotelSettingsDto Settings { get; set; } = new();

        // Public info (có thể show cho guest)
        public HotelPublicSettingsDto PublicSettings { get; set; } = new();

        public List<HotelImageDto> Images { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
        public List<ReviewDto> RecentReviews { get; set; } = new();
    }

    // Public settings - Hiển thị cho khách
    public class HotelPublicSettingsDto
    {
        public string CheckInTime { get; set; } = "14:00";
        public string CheckOutTime { get; set; } = "12:00";
        public int MaxGuestsPerRoom { get; set; } = 4;
        public bool AllowExtraBed { get; set; } = false;
        public decimal? ExtraBedPrice { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public string? SmokingPolicy { get; set; }
        public bool EnableStripePayment { get; set; } = true;
        public bool EnablePayAtHotel { get; set; } = true;
    }

    public class HotelSearchRequestDto : PaginationRequestDto
    {
        public string? Query { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? MinStarRating { get; set; }
        public int? MaxStarRating { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public int? NumberOfRooms { get; set; }
        public List<Guid>? AmenityIds { get; set; }
        public float? MinRating { get; set; }
    }

    public class HotelSearchResultDto : HotelDto
    {
        public int AvailableRooms { get; set; }
        public decimal? LowestAvailablePrice { get; set; }
    }
}
