using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class SendNotificationDto
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PromotionId { get; set; }
    }

    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.SystemAlert;
        public string? ActionUrl { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PromotionId { get; set; }
    }

    public class MarkNotificationsReadDto
    {
        public List<Guid> NotificationIds { get; set; } = new();
    }

    // Response DTOs
    public class NotificationDto : BaseDto
    {
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationStatus Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PromotionId { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead => ReadAt.HasValue;
    }

    public class NotificationSummaryDto
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public List<NotificationDto> Notifications { get; set; } = new();
    }

    public class UnreadCountDto
    {
        public int Count { get; set; }
    }
}
