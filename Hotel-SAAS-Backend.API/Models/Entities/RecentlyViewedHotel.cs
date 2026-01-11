namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class RecentlyViewedHotel : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime ViewedAt { get; set; }
        public int ViewCount { get; set; } = 1;

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}
