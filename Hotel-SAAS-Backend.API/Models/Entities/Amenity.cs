using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Amenity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public AmenityType Type { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
