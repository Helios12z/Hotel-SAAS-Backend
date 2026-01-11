namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class HotelAmenity : BaseEntity
    {
        public Guid HotelId { get; set; }
        public Guid AmenityId { get; set; }
        public bool IsComplimentary { get; set; } = true;
        public decimal? AdditionalCost { get; set; }
        public string? OperatingHours { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Hotel Hotel { get; set; } = null!;
        public virtual Amenity Amenity { get; set; } = null!;
    }
}
