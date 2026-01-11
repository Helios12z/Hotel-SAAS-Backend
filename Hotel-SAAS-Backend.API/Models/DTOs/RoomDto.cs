using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateRoomDto
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
        public string? Description { get; set; }
        public bool HasView { get; set; }
        public string? ViewDescription { get; set; }
        public bool SmokingAllowed { get; set; }
        public bool IsPetFriendly { get; set; }
        public bool HasConnectingRoom { get; set; }
        public Guid? ConnectingRoomId { get; set; }
        public bool IsAccessible { get; set; }
        public string? AccessibilityFeatures { get; set; }
        public List<Guid> AmenityIds { get; set; } = new();
    }

    public class UpdateRoomDto
    {
        public string? Floor { get; set; }
        public RoomType? Type { get; set; }
        public BedType? BedType { get; set; }
        public int? NumberOfBeds { get; set; }
        public int? MaxOccupancy { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? WeekendPrice { get; set; }
        public decimal? HolidayPrice { get; set; }
        public int? SizeInSquareMeters { get; set; }
        public string? Description { get; set; }
        public bool? HasView { get; set; }
        public string? ViewDescription { get; set; }
        public bool? SmokingAllowed { get; set; }
        public bool? IsPetFriendly { get; set; }
        public bool? HasConnectingRoom { get; set; }
        public Guid? ConnectingRoomId { get; set; }
        public bool? IsAccessible { get; set; }
        public string? AccessibilityFeatures { get; set; }
    }

    // Response DTOs
    public class RoomDto : BaseDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string? Floor { get; set; }
        public RoomType Type { get; set; }
        public BedType BedType { get; set; }
        public int NumberOfBeds { get; set; }
        public int MaxOccupancy { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? WeekendPrice { get; set; }
        public decimal? HolidayPrice { get; set; }
        public int SizeInSquareMeters { get; set; }
        public RoomStatus Status { get; set; }
        public string? Description { get; set; }
        public bool HasView { get; set; }
        public string? ViewDescription { get; set; }
        public bool IsAccessible { get; set; }
    }

    public class RoomDetailDto : RoomDto
    {
        public string? ImageUrl { get; set; }
        public bool SmokingAllowed { get; set; }
        public bool IsPetFriendly { get; set; }
        public bool HasConnectingRoom { get; set; }
        public Guid? ConnectingRoomId { get; set; }
        public string? AccessibilityFeatures { get; set; }
        public List<AmenityDto> Amenities { get; set; } = new();
        public List<RoomImageDto> Images { get; set; } = new();
    }

    public class RoomImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}
