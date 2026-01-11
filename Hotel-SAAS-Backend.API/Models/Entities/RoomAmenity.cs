namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class RoomAmenity : BaseEntity
    {
        public Guid RoomId { get; set; }
        public Guid AmenityId { get; set; }
        public bool IsComplimentary { get; set; } = true;
        public decimal? AdditionalCost { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Room Room { get; set; } = null!;
        public virtual Amenity Amenity { get; set; } = null!;
    }
}
