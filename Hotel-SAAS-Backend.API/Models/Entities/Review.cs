using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Review : BaseEntity
    {
        public Guid HotelId { get; set; }
        public Guid GuestId { get; set; }
        public Guid? BookingId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
        public int? CleanlinessRating { get; set; }
        public int? ServiceRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        public bool IsVerified { get; set; } = false; // Verified stay
        public DateTime? StayDate { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? ManagementResponse { get; set; }
        public DateTime? ManagementResponseAt { get; set; }
        public int HelpfulCount { get; set; } = 0;
        public int NotHelpfulCount { get; set; } = 0;

        // Vector embedding for AI review analysis
        public float[]? Embedding { get; set; }
        public float[]? SentimentEmbedding { get; set; }

        // Navigation properties
        public virtual Hotel Hotel { get; set; } = null!;
        public virtual User Guest { get; set; } = null!;
        public virtual Booking? Booking { get; set; }
        public virtual ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
    }
}
