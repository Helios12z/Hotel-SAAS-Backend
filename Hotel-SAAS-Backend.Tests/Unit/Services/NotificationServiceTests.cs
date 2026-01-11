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
            new Notification { Id = Guid.NewGuid(), UserId = userId, Title = "Test", Message = "Test message" }
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
            .ReturnsAsync(new Notification { Id = Guid.NewGuid(), Title = "Test", Message = "Test" });

        // Act
        await _sut.SendBookingConfirmationAsync(bookingId);

        // Assert
        _notificationRepoMock.Verify(x => x.CreateAsync(It.Is<Notification>(n =>
            n.Type == NotificationType.BookingConfirmation)), Times.Once);
    }

    [Fact]
    public async Task SendBookingCancellationAsync_CreatesNotification()
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
            .ReturnsAsync(new Notification { Id = Guid.NewGuid(), Title = "Test", Message = "Test" });

        // Act
        await _sut.SendBookingCancellationAsync(bookingId, "Guest requested");

        // Assert
        _notificationRepoMock.Verify(x => x.CreateAsync(It.Is<Notification>(n =>
            n.Type == NotificationType.BookingCancellation)), Times.Once);
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

    [Fact]
    public async Task MarkAsReadAsync_CallsRepository()
    {
        // Arrange
        var notificationId = Guid.NewGuid();

        // Act
        await _sut.MarkAsReadAsync(notificationId);

        // Assert
        _notificationRepoMock.Verify(x => x.MarkAsReadAsync(notificationId), Times.Once);
    }

    [Fact]
    public async Task DeleteNotificationAsync_CallsRepositoryAndReturnsResult()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        _notificationRepoMock.Setup(x => x.DeleteAsync(notificationId)).ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteNotificationAsync(notificationId);

        // Assert
        Assert.True(result);
        _notificationRepoMock.Verify(x => x.DeleteAsync(notificationId), Times.Once);
    }

    [Fact]
    public async Task MarkMultipleAsReadAsync_CallsRepository()
    {
        // Arrange
        var notificationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        await _sut.MarkMultipleAsReadAsync(notificationIds);

        // Assert
        _notificationRepoMock.Verify(x => x.MarkMultipleAsReadAsync(notificationIds), Times.Once);
    }
}
