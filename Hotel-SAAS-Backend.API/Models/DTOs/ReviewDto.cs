namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateReviewDto
    {
        public Guid HotelId { get; set; }
        public Guid? BookingId { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int? CleanlinessRating { get; set; }
        public int? ServiceRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateReviewDto
    {
        public int? Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public int? CleanlinessRating { get; set; }
        public int? ServiceRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
    }

    // Response DTOs
    public class ReviewDto : BaseDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public Guid GuestId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string? GuestAvatarUrl { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int? CleanlinessRating { get; set; }
        public int? ServiceRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? StayDate { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? ManagementResponse { get; set; }
        public List<ReviewImageDto> Images { get; set; } = new();
    }

    public class ReviewImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }
}
