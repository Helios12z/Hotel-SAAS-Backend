namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class HotelImage : BaseEntity
    {
        public Guid HotelId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;
        public string? Category { get; set; } // Lobby, Room, Pool, Restaurant, etc.

        // Navigation properties
        public virtual Hotel Hotel { get; set; } = null!;
    }
}
