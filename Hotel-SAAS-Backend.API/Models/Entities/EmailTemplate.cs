namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class EmailTemplate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;  // Unique name
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;  // HTML template
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Placeholders: {{GuestName}}, {{HotelName}}, {{CheckInDate}}, etc.
    }
}
