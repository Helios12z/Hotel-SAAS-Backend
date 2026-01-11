using Microsoft.EntityFrameworkCore;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class GuestProfileServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRecentlyViewedRepository> _recentlyViewedRepoMock;
    private readonly GuestProfileService _sut;
    private readonly Guid _userId;
    private readonly Guid _hotelId;

    public GuestProfileServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _userRepoMock = new Mock<IUserRepository>();
        _recentlyViewedRepoMock = new Mock<IRecentlyViewedRepository>();

        _sut = new GuestProfileService(
            _userRepoMock.Object,
            _recentlyViewedRepoMock.Object,
            _context);

        _userId = Guid.NewGuid();
        _hotelId = Guid.NewGuid();
        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = _userId,
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            PreferredCurrency = "USD",
            PreferredLanguage = "en"
        };

        var brand = new Brand { Id = Guid.NewGuid(), Name = "Test Brand" };
        var hotel = new Hotel { Id = _hotelId, Name = "Test Hotel", BrandId = brand.Id, Brand = brand, City = "Paris" };

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            GuestId = _userId,
            Guest = user,
            HotelId = hotel.Id,
            Hotel = hotel,
            ConfirmationNumber = "BK123",
            CheckInDate = DateTime.UtcNow.AddDays(-5),
            CheckOutDate = DateTime.UtcNow.AddDays(-3),
            Status = BookingStatus.CheckedOut,
            TotalAmount = 500
        };

        _context.Users.Add(user);
        _context.Brands.Add(brand);
        _context.Hotels.Add(hotel);
        _context.Bookings.Add(booking);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProfileAsync_WhenUserExists_ReturnsProfile()
    {
        // Arrange
        var user = await _context.Users.FindAsync(_userId);
        _userRepoMock.Setup(x => x.GetByIdAsync(_userId)).ReturnsAsync(user);

        // Act
        var result = await _sut.GetProfileAsync(_userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_userId, result.Id);
        Assert.Equal("test@test.com", result.Email);
        Assert.Equal(1, result.TotalBookings);
        Assert.Equal(1, result.CompletedStays);
    }

    [Fact]
    public async Task GetProfileAsync_WhenUserNotExists_ReturnsNull()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetProfileAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingHistoryAsync_ReturnsFilteredBookings()
    {
        // Arrange
        var filter = new BookingHistoryFilterDto
        {
            Status = BookingStatus.CheckedOut,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _sut.GetBookingHistoryAsync(_userId, filter);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bookings.Items);
        Assert.Equal(1, result.Stats.CompletedBookings);
        Assert.Equal(500, result.Stats.TotalSpent);
        Assert.Equal("Paris", result.Stats.MostVisitedCity);
    }

    [Fact]
    public async Task GetBookingHistoryAsync_WithDateFilter_ReturnsFilteredResults()
    {
        // Arrange
        var filter = new BookingHistoryFilterDto
        {
            FromDate = DateTime.UtcNow.AddDays(-10),
            ToDate = DateTime.UtcNow,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _sut.GetBookingHistoryAsync(_userId, filter);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bookings.Items);
    }

    [Fact]
    public async Task GetRecentlyViewedAsync_ReturnsRecentlyViewed()
    {
        // Arrange
        var hotel = await _context.Hotels.FirstAsync();
        var recentlyViewed = new List<RecentlyViewedHotel>
        {
            new RecentlyViewedHotel
            {
                UserId = _userId,
                HotelId = hotel.Id,
                Hotel = hotel,
                ViewedAt = DateTime.UtcNow,
                ViewCount = 3
            }
        };
        _recentlyViewedRepoMock.Setup(x => x.GetByUserIdAsync(_userId, 10)).ReturnsAsync(recentlyViewed);

        // Act
        var result = await _sut.GetRecentlyViewedAsync(_userId);

        // Assert
        Assert.Single(result);
        Assert.Equal(3, result.First().ViewCount);
    }

    [Fact]
    public async Task UpdatePreferencesAsync_UpdatesAndReturnsProfile()
    {
        // Arrange
        var user = await _context.Users.FindAsync(_userId);
        _userRepoMock.Setup(x => x.GetByIdAsync(_userId)).ReturnsAsync(user);
        _userRepoMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user!);

        var dto = new UpdateGuestPreferencesDto
        {
            PreferredCurrency = "EUR",
            PreferredLanguage = "de",
            EmailNotificationsEnabled = false
        };

        // Act
        var result = await _sut.UpdatePreferencesAsync(_userId, dto);

        // Assert
        Assert.NotNull(result);
        _userRepoMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task TrackHotelViewAsync_CallsRepository()
    {
        // Act
        await _sut.TrackHotelViewAsync(_userId, _hotelId);

        // Assert
        _recentlyViewedRepoMock.Verify(x => x.AddOrUpdateViewAsync(_userId, _hotelId), Times.Once);
    }

    [Fact]
    public async Task ClearRecentlyViewedAsync_CallsRepository()
    {
        // Act
        await _sut.ClearRecentlyViewedAsync(_userId);

        // Assert
        _recentlyViewedRepoMock.Verify(x => x.ClearUserHistoryAsync(_userId), Times.Once);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
