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

        // ============ Time Config ============
        public string CheckInTime { get; set; } = "14:00";
        public string CheckOutTime { get; set; } = "12:00";

        // ============ Guest Config ============
        public int MaxAdultsPerRoom { get; set; } = 2;
        public int MaxChildrenPerRoom { get; set; } = 1;
        public int MaxGuestsPerRoom { get; set; } = 4; // adults + children
        public bool AllowExtraBed { get; set; } = false;
        public decimal? ExtraBedPrice { get; set; }

        // ============ Booking Rules ============
        public int MinNights { get; set; } = 1;
        public int MaxNights { get; set; } = 30;
        public int MinAdvanceBookingHours { get; set; } = 24; // Đặt trước tối thiểu
        public int MaxAdvanceBookingDays { get; set; } = 365; // Đặt trước tối đa

        // ============ Payment Config ============
        public bool EnableStripePayment { get; set; } = true;  // Pay online via Stripe
        public bool EnablePayAtHotel { get; set; } = true;     // Pay at front desk
        public string? StripeAccountId { get; set; }           // Hotel's Stripe connected account

        // ============ Tax & Fee Config ============
        public decimal TaxRate { get; set; } = 0.10m;           // 10% VAT
        public decimal ServiceFeeRate { get; set; } = 0.05m;    // 5% Platform fee

        // ============ Policies ============
        public string? CancellationPolicy { get; set; }
        public string? ChildPolicy { get; set; }
        public string? PetPolicy { get; set; }
        public string? SmokingPolicy { get; set; }

        // ============ Hotel Info ============
        public int? TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        public DateTime? YearBuilt { get; set; }
        public DateTime? YearRenovated { get; set; }
        public decimal? CommissionRate { get; set; } // Platform commission (deprecated, use SubscriptionPlan)
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
