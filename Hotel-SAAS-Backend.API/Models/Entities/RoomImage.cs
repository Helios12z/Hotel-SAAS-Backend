namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class RoomImage : BaseEntity
    {
        public Guid RoomId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;

        // Navigation properties
        public virtual Room Room { get; set; } = null!;
    }
}
