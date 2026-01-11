namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class AddToWishlistDto
    {
        public Guid HotelId { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateWishlistDto
    {
        public string? Note { get; set; }
    }

    // Response DTOs
    public class WishlistDto : BaseDto
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public string? Note { get; set; }
        public HotelDto Hotel { get; set; } = null!;
    }

    public class WishlistSummaryDto
    {
        public int TotalItems { get; set; }
        public List<WishlistDto> Items { get; set; } = new();
    }
}
