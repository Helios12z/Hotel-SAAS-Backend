using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Hotel : BaseEntity
    {
        public Guid BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
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
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public string? TaxId { get; set; }
        public string? CheckInTime { get; set; } = "14:00";
        public string? CheckOutTime { get; set; } = "12:00";
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public string? SmokingPolicy { get; set; }
        public int? TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        public DateTime? YearBuilt { get; set; }
        public DateTime? YearRenovated { get; set; }
        public decimal? CommissionRate { get; set; } // Platform commission
        public float? AverageRating { get; set; }
        public int ReviewCount { get; set; } = 0;

        // Vector embedding for AI-powered search and recommendations
        public float[]? Embedding { get; set; }

        // Navigation properties
        public virtual Brand Brand { get; set; } = null!;
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<HotelAmenity> Amenities { get; set; } = new List<HotelAmenity>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
    }
}
