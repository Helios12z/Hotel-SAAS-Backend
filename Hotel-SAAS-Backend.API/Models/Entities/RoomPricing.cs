namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class RoomPricing : BaseEntity
    {
        public Guid RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; } = "USD";
        public bool IsActive { get; set; } = true;
        public string? Reason { get; set; } // Holiday, Event, Season, etc.
        public int? MinimumStay { get; set; }
        public int? MaximumStay { get; set; }

        // Navigation properties
        public virtual Room Room { get; set; } = null!;
    }
}
