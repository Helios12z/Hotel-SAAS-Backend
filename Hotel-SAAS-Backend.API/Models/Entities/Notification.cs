using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        
        // Reference to related entity
        public Guid? BookingId { get; set; }
        public Guid? PromotionId { get; set; }
        
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        
        // For email
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }
        
        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Booking? Booking { get; set; }
        public virtual Promotion? Promotion { get; set; }
    }
}
