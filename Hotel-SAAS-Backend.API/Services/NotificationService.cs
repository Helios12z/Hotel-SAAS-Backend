using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class NotificationService(
        INotificationRepository notificationRepository,
        IBookingRepository bookingRepository,
        IEmailService emailService) : INotificationService
    {
        public async Task<NotificationSummaryDto> GetUserNotificationsAsync(Guid userId, int limit = 50)
        {
            var notifications = await notificationRepository.GetByUserIdAsync(userId, limit);
            var items = notifications.Select(Mapper.ToDto).ToList();
            var unreadCount = await notificationRepository.GetUnreadCountAsync(userId);

            return new NotificationSummaryDto
            {
                TotalCount = items.Count,
                UnreadCount = unreadCount,
                Notifications = items
            };
        }

        public async Task<UnreadCountDto> GetUnreadCountAsync(Guid userId)
        {
            var count = await notificationRepository.GetUnreadCountAsync(userId);
            return new UnreadCountDto { Count = count };
        }

        public async Task<NotificationDto> CreateNotificationAsync(SendNotificationDto dto)
        {
            var notification = Mapper.ToEntity(dto);
            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;

            var created = await notificationRepository.CreateAsync(notification);
            return Mapper.ToDto(created);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task MarkMultipleAsReadAsync(List<Guid> notificationIds)
        {
            await notificationRepository.MarkMultipleAsReadAsync(notificationIds);
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            return await notificationRepository.DeleteAsync(notificationId);
        }

        public async Task SendBookingConfirmationAsync(Guid bookingId)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) return;

            // Create in-app notification
            var notification = new SendNotificationDto
            {
                UserId = booking.GuestId,
                Type = NotificationType.BookingConfirmation,
                Channel = NotificationChannel.InApp,
                Title = "Booking Confirmed!",
                Message = $"Your booking at {booking.Hotel?.Name} has been confirmed. " +
                          $"Confirmation number: {booking.ConfirmationNumber}",
                ActionUrl = $"/bookings/{booking.Id}",
                BookingId = bookingId
            };
            await CreateNotificationAsync(notification);

            // Send email (fire and forget for MVP)
            _ = emailService.SendBookingConfirmationAsync(bookingId);
        }

        public async Task SendBookingCancellationAsync(Guid bookingId, string? reason)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) return;

            var notification = new SendNotificationDto
            {
                UserId = booking.GuestId,
                Type = NotificationType.BookingCancellation,
                Channel = NotificationChannel.InApp,
                Title = "Booking Cancelled",
                Message = $"Your booking at {booking.Hotel?.Name} has been cancelled.",
                ActionUrl = $"/bookings/{booking.Id}",
                BookingId = bookingId
            };
            await CreateNotificationAsync(notification);

            _ = emailService.SendBookingCancellationAsync(bookingId, reason);
        }
    }
}
