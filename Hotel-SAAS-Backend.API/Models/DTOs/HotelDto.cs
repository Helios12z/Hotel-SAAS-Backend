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
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public int? TotalRooms { get; set; }
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
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public int? TotalRooms { get; set; }
        public decimal? CommissionRate { get; set; }
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
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public int? TotalRooms { get; set; }
        public List<HotelImageDto> Images { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
        public List<ReviewDto> RecentReviews { get; set; } = new();
    }
}
