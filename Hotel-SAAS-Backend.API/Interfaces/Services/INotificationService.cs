using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationSummaryDto> GetUserNotificationsAsync(Guid userId, int limit = 50);
        Task<UnreadCountDto> GetUnreadCountAsync(Guid userId);
        Task<NotificationDto> CreateNotificationAsync(SendNotificationDto dto);
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task MarkMultipleAsReadAsync(List<Guid> notificationIds);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        
        // Booking-related notifications
        Task SendBookingConfirmationAsync(Guid bookingId);
        Task SendBookingCancellationAsync(Guid bookingId, string? reason);
        
        // Admin notifications
        Task NotifyAdminsAsync(string title, string message, NotificationType type, string? actionUrl = null);
    }
}
