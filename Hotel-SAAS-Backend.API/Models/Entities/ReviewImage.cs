namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class ReviewImage : BaseEntity
    {
        public Guid ReviewId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public virtual Review Review { get; set; } = null!;
    }
}
