namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public string? Note { get; set; }  // Optional: user's note
        
        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}
