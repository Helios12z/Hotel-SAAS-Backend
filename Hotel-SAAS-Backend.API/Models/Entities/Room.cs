using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Room : BaseEntity
    {
        public Guid HotelId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string? Floor { get; set; }
        public RoomType Type { get; set; }
        public BedType BedType { get; set; }
        public int NumberOfBeds { get; set; } = 1;
        public int MaxOccupancy { get; set; } = 2;
        public decimal BasePrice { get; set; }
        public decimal? WeekendPrice { get; set; }
        public decimal? HolidayPrice { get; set; }
        public int SizeInSquareMeters { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        public string? Description { get; set; }
        public bool HasView { get; set; }
        public string? ViewDescription { get; set; } // Ocean view, City view, Garden view
        public bool SmokingAllowed { get; set; }
        public bool IsPetFriendly { get; set; }
        public bool HasConnectingRoom { get; set; }
        public Guid? ConnectingRoomId { get; set; }
        public bool IsAccessible { get; set; } // Wheelchair accessible
        public string? AccessibilityFeatures { get; set; }

        // Vector embedding for AI-powered room recommendations
        public float[]? Embedding { get; set; }

        // Navigation properties
        public virtual Hotel Hotel { get; set; } = null!;
        public virtual Room? ConnectingRoom { get; set; }
        public virtual ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
        public virtual ICollection<RoomAmenity> Amenities { get; set; } = new List<RoomAmenity>();
        public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    }
}
