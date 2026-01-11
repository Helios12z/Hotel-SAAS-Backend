# Phase 4: Notifications

## ?? M?c tiêu
G?i email và in-app notifications cho booking events.

## ?? Tasks

### Task 4.1: Create Enums

#### File: `Models/Enums/NotificationType.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.Enums
{
    public enum NotificationType
    {
        BookingConfirmation,
        BookingCancellation,
        BookingReminder,
        CheckInReminder,
        CheckOutReminder,
        PaymentReceived,
        PaymentFailed,
        ReviewRequest,
        PromotionAlert,
        SystemAlert
    }

    public enum NotificationChannel
    {
        InApp,
        Email,
        SMS,
        Push
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Read,
        Failed
    }
}
```

---

### Task 4.2: Create Entities

#### File: `Models/Entities/Notification.cs`
```csharp
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
        public string? ActionUrl { get; set; }  // Link ?? click vào
        
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
```

#### File: `Models/Entities/EmailTemplate.cs`
```csharp
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
```

---

### Task 4.3: Update DbContext

#### File: `Data/ApplicationDbContext.cs`
```csharp
// Thêm DbSets
public DbSet<Notification> Notifications { get; set; }
public DbSet<EmailTemplate> EmailTemplates { get; set; }

// Trong OnModelCreating:

// Notification Configuration
modelBuilder.Entity<Notification>(entity =>
{
    entity.ToTable("notifications");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
    entity.Property(e => e.Message).IsRequired();
    entity.Property(e => e.ActionUrl).HasMaxLength(1000);
    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.Type);
    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => e.CreatedAt);

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Booking)
        .WithMany()
        .HasForeignKey(e => e.BookingId)
        .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.Promotion)
        .WithMany()
        .HasForeignKey(e => e.PromotionId)
        .OnDelete(DeleteBehavior.SetNull);
});

// EmailTemplate Configuration
modelBuilder.Entity<EmailTemplate>(entity =>
{
    entity.ToTable("email_templates");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
    entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
    entity.Property(e => e.Body).IsRequired();
    entity.HasIndex(e => e.Name).IsUnique();
});
```

---

### Task 4.4: Create DTOs

#### File: `Models/DTOs/NotificationDto.cs`
```csharp
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
```

---

### Task 4.5: Add Mapper Methods

#### File: `Mapping/Mapper.cs`
```csharp
#region Notification Mappings

public static NotificationDto ToDto(Notification entity)
{
    return new NotificationDto
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Type = entity.Type,
        Channel = entity.Channel,
        Status = entity.Status,
        Title = entity.Title,
        Message = entity.Message,
        ActionUrl = entity.ActionUrl,
        BookingId = entity.BookingId,
        PromotionId = entity.PromotionId,
        ReadAt = entity.ReadAt
    };
}

public static Notification ToEntity(SendNotificationDto dto)
{
    return new Notification
    {
        UserId = dto.UserId,
        Type = dto.Type,
        Channel = dto.Channel,
        Status = NotificationStatus.Pending,
        Title = dto.Title,
        Message = dto.Message,
        ActionUrl = dto.ActionUrl,
        BookingId = dto.BookingId,
        PromotionId = dto.PromotionId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}

#endregion
```

---

### Task 4.6: Create Repository

#### File: `Interfaces/Repositories/INotificationRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task MarkMultipleAsReadAsync(List<Guid> notificationIds);
    }
}
```

#### File: `Repositories/NotificationRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class NotificationRepository(ApplicationDbContext context) 
        : BaseRepository<Notification>(context), INotificationRepository
    {
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsDeleted && n.ReadAt == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(n => n.UserId == userId && !n.IsDeleted && n.ReadAt == null);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null && notification.ReadAt == null)
            {
                notification.ReadAt = DateTime.UtcNow;
                notification.Status = NotificationStatus.Read;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            await _dbSet
                .Where(n => n.UserId == userId && n.ReadAt == null && !n.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.ReadAt, now)
                    .SetProperty(n => n.Status, NotificationStatus.Read)
                    .SetProperty(n => n.UpdatedAt, now));
        }

        public async Task MarkMultipleAsReadAsync(List<Guid> notificationIds)
        {
            var now = DateTime.UtcNow;
            await _dbSet
                .Where(n => notificationIds.Contains(n.Id) && n.ReadAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.ReadAt, now)
                    .SetProperty(n => n.Status, NotificationStatus.Read)
                    .SetProperty(n => n.UpdatedAt, now));
        }
    }
}
```

---

### Task 4.7: Create Email Service (Basic)

#### File: `Models/Options/EmailOptions.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.Options
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Hotel SAAS";
        public bool EnableSsl { get; set; } = true;
    }
}
```

#### File: `Interfaces/Services/IEmailService.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendBookingConfirmationAsync(Guid bookingId);
        Task<bool> SendBookingCancellationAsync(Guid bookingId, string? reason);
        Task<bool> SendCheckInReminderAsync(Guid bookingId);
    }
}
```

#### File: `Services/EmailService.cs`
```csharp
using System.Net;
using System.Net.Mail;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Options;
using Microsoft.Extensions.Options;

namespace Hotel_SAAS_Backend.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailOptions> options,
            IBookingRepository bookingRepository,
            ILogger<EmailService> logger)
        {
            _options = options.Value;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Skip if SMTP not configured
                if (string.IsNullOrEmpty(_options.SmtpHost))
                {
                    _logger.LogWarning("SMTP not configured, skipping email send");
                    return false;
                }

                using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
                    Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword),
                    EnableSsl = _options.EnableSsl
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_options.FromEmail, _options.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                return false;
            }
        }

        public async Task<bool> SendBookingConfirmationAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.GuestEmail))
                return false;

            var subject = $"Booking Confirmed - {booking.ConfirmationNumber}";
            var body = BuildBookingConfirmationEmail(booking);

            return await SendEmailAsync(booking.GuestEmail, subject, body);
        }

        public async Task<bool> SendBookingCancellationAsync(Guid bookingId, string? reason)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.GuestEmail))
                return false;

            var subject = $"Booking Cancelled - {booking.ConfirmationNumber}";
            var body = BuildBookingCancellationEmail(booking, reason);

            return await SendEmailAsync(booking.GuestEmail, subject, body);
        }

        public async Task<bool> SendCheckInReminderAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.GuestEmail))
                return false;

            var subject = $"Check-in Reminder - {booking.Hotel?.Name}";
            var body = BuildCheckInReminderEmail(booking);

            return await SendEmailAsync(booking.GuestEmail, subject, body);
        }

        // Simple HTML email templates
        private static string BuildBookingConfirmationEmail(Models.Entities.Booking booking)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Confirmed!</h2>
                <p>Dear {booking.GuestName},</p>
                <p>Your booking has been confirmed.</p>
                <div style='background: #f5f5f5; padding: 20px; margin: 20px 0;'>
                    <p><strong>Confirmation Number:</strong> {booking.ConfirmationNumber}</p>
                    <p><strong>Hotel:</strong> {booking.Hotel?.Name}</p>
                    <p><strong>Check-in:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                    <p><strong>Check-out:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</p>
                    <p><strong>Total Amount:</strong> {booking.Currency} {booking.TotalAmount:N2}</p>
                </div>
                <p>Thank you for choosing us!</p>
            </body>
            </html>";
        }

        private static string BuildBookingCancellationEmail(Models.Entities.Booking booking, string? reason)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Booking Cancelled</h2>
                <p>Dear {booking.GuestName},</p>
                <p>Your booking has been cancelled.</p>
                <div style='background: #f5f5f5; padding: 20px; margin: 20px 0;'>
                    <p><strong>Confirmation Number:</strong> {booking.ConfirmationNumber}</p>
                    <p><strong>Hotel:</strong> {booking.Hotel?.Name}</p>
                    {(string.IsNullOrEmpty(reason) ? "" : $"<p><strong>Reason:</strong> {reason}</p>")}
                </div>
                <p>If you have any questions, please contact us.</p>
            </body>
            </html>";
        }

        private static string BuildCheckInReminderEmail(Models.Entities.Booking booking)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Check-in Reminder</h2>
                <p>Dear {booking.GuestName},</p>
                <p>This is a reminder that your check-in is coming up!</p>
                <div style='background: #f5f5f5; padding: 20px; margin: 20px 0;'>
                    <p><strong>Hotel:</strong> {booking.Hotel?.Name}</p>
                    <p><strong>Address:</strong> {booking.Hotel?.Address}, {booking.Hotel?.City}</p>
                    <p><strong>Check-in Date:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                    <p><strong>Check-in Time:</strong> {booking.Hotel?.CheckInTime ?? "14:00"}</p>
                </div>
                <p>We look forward to welcoming you!</p>
            </body>
            </html>";
        }
    }
}
```

---

### Task 4.8: Create Notification Service

#### File: `Interfaces/Services/INotificationService.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationSummaryDto> GetUserNotificationsAsync(Guid userId, int limit = 50);
        Task<UnreadCountDto> GetUnreadCountAsync(Guid userId);
        Task<NotificationDto> CreateNotificationAsync(SendNotificationDto dto);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task MarkMultipleAsReadAsync(List<Guid> notificationIds);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        
        // Booking-related notifications
        Task SendBookingConfirmationAsync(Guid bookingId);
        Task SendBookingCancellationAsync(Guid bookingId, string? reason);
    }
}
```

#### File: `Services/NotificationService.cs`
```csharp
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
```

---

### Task 4.9: Create Controller

#### File: `Controllers/NotificationsController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<NotificationSummaryDto>>> GetNotifications(
            [FromQuery] int limit = 50)
        {
            var result = await _notificationService.GetUserNotificationsAsync(GetUserId(), limit);
            return Ok(new ApiResponseDto<NotificationSummaryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponseDto<UnreadCountDto>>> GetUnreadCount()
        {
            var result = await _notificationService.GetUnreadCountAsync(GetUserId());
            return Ok(new ApiResponseDto<UnreadCountDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost("{id}/read")]
        public async Task<ActionResult<ApiResponseDto<bool>>> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Marked as read",
                Data = true
            });
        }

        [HttpPost("read-all")]
        public async Task<ActionResult<ApiResponseDto<bool>>> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(GetUserId());
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "All notifications marked as read",
                Data = true
            });
        }

        [HttpPost("read-multiple")]
        public async Task<ActionResult<ApiResponseDto<bool>>> MarkMultipleAsRead(
            [FromBody] MarkNotificationsReadDto dto)
        {
            await _notificationService.MarkMultipleAsReadAsync(dto.NotificationIds);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Notifications marked as read",
                Data = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteNotification(Guid id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Notification deleted" : "Notification not found",
                Data = result
            });
        }
    }
}
```

---

### Task 4.10: Integrate with BookingService

#### File: `Services/BookingService.cs` - Update methods
```csharp
public class BookingService(
    IBookingRepository bookingRepository,
    IRoomRepository roomRepository,
    INotificationService notificationService) : IBookingService  // Thêm dependency
{
    // Update ConfirmBookingAsync
    public async Task<bool> ConfirmBookingAsync(Guid id)
    {
        var booking = await bookingRepository.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Confirmed;
        booking.ConfirmedAt = DateTime.UtcNow;

        await bookingRepository.UpdateAsync(booking);
        
        // Send notification
        await notificationService.SendBookingConfirmationAsync(id);
        
        return true;
    }

    // Update CancelBookingAsync
    public async Task<bool> CancelBookingAsync(Guid id, string? reason)
    {
        var booking = await bookingRepository.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        booking.CancellationReason = reason;

        await bookingRepository.UpdateAsync(booking);
        
        // Send notification
        await notificationService.SendBookingCancellationAsync(id, reason);
        
        return true;
    }
}
```

---

### Task 4.11: Update Configuration

#### File: `appsettings.json`
```json
{
  "Email": {
    "SmtpHost": "",
    "SmtpPort": 587,
    "SmtpUsername": "",
    "SmtpPassword": "",
    "FromEmail": "noreply@hotelsaas.com",
    "FromName": "Hotel SAAS",
    "EnableSsl": true
  }
}
```

---

### Task 4.12: Register in Program.cs

```csharp
// Configuration
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

// Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
```

---

### Task 4.13: Create Migration

```bash
dotnet ef migrations add AddNotifications
dotnet ef database update
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/NotificationServiceTests.cs`

```csharp
using Xunit;
using Moq;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _bookingRepoMock = new Mock<IBookingRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _sut = new NotificationService(
            _notificationRepoMock.Object,
            _bookingRepoMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ReturnsNotificationSummary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            new Notification { Id = Guid.NewGuid(), UserId = userId, Title = "Test", Message = "Test" }
        };
        _notificationRepoMock.Setup(x => x.GetByUserIdAsync(userId, 50)).ReturnsAsync(notifications);
        _notificationRepoMock.Setup(x => x.GetUnreadCountAsync(userId)).ReturnsAsync(1);

        // Act
        var result = await _sut.GetUserNotificationsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.UnreadCount);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ReturnsCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _notificationRepoMock.Setup(x => x.GetUnreadCountAsync(userId)).ReturnsAsync(5);

        // Act
        var result = await _sut.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task SendBookingConfirmationAsync_CreatesNotificationAndSendsEmail()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            GuestId = Guid.NewGuid(),
            ConfirmationNumber = "BK123",
            Hotel = new Hotel { Name = "Test Hotel" }
        };
        _bookingRepoMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);
        _notificationRepoMock.Setup(x => x.CreateAsync(It.IsAny<Notification>()))
            .ReturnsAsync(new Notification { Id = Guid.NewGuid() });

        // Act
        await _sut.SendBookingConfirmationAsync(bookingId);

        // Assert
        _notificationRepoMock.Verify(x => x.CreateAsync(It.Is<Notification>(n => 
            n.Type == NotificationType.BookingConfirmation)), Times.Once);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_CallsRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _sut.MarkAllAsReadAsync(userId);

        // Assert
        _notificationRepoMock.Verify(x => x.MarkAllAsReadAsync(userId), Times.Once);
    }
}
```

---

## ? Checklist

- [ ] Enums `NotificationType`, `NotificationChannel`, `NotificationStatus` ?ã t?o
- [ ] Entity `Notification` ?ã t?o
- [ ] Entity `EmailTemplate` ?ã t?o
- [ ] DbContext ?ã update
- [ ] DTOs ?ã t?o (`NotificationDto.cs`)
- [ ] `EmailOptions` ?ã t?o
- [ ] Mapper methods ?ã thêm
- [ ] `INotificationRepository` ?ã t?o
- [ ] `NotificationRepository` ?ã implement
- [ ] `IEmailService` ?ã t?o
- [ ] `EmailService` ?ã implement
- [ ] `INotificationService` ?ã t?o
- [ ] `NotificationService` ?ã implement
- [ ] `NotificationsController` ?ã t?o
- [ ] `BookingService` ?ã integrate v?i notifications
- [ ] `appsettings.json` ?ã update v?i Email config
- [ ] Registered trong `Program.cs`
- [ ] Migration ?ã t?o
- [ ] Unit tests ?ã vi?t
- [ ] Build passes

---

## ?? API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/notifications` | User | Get user notifications |
| GET | `/api/notifications/unread-count` | User | Get unread count |
| POST | `/api/notifications/{id}/read` | User | Mark as read |
| POST | `/api/notifications/read-all` | User | Mark all as read |
| POST | `/api/notifications/read-multiple` | User | Mark multiple as read |
| DELETE | `/api/notifications/{id}` | User | Delete notification |
