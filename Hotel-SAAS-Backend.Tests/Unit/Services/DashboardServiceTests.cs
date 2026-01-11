using Microsoft.EntityFrameworkCore;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class DashboardServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DashboardService _sut;
    private readonly Guid _hotelId;
    private readonly Guid _brandId;

    public DashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new DashboardService(_context);
        _hotelId = Guid.NewGuid();
        _brandId = Guid.NewGuid();

        SeedTestData();
    }

    private void SeedTestData()
    {
        var brand = new Brand { Id = _brandId, Name = "Test Brand" };
        var hotel = new Hotel { Id = _hotelId, Name = "Test Hotel", BrandId = brand.Id, Brand = brand };

        var room1 = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = _hotelId,
            RoomNumber = "101",
            Status = RoomStatus.Available,
            BasePrice = 100,
            Type = RoomType.Standard
        };
        var room2 = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = _hotelId,
            RoomNumber = "102",
            Status = RoomStatus.Occupied,
            BasePrice = 150,
            Type = RoomType.Deluxe
        };

        var guest = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "Guest",
            PasswordHash = "hash"
        };

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = _hotelId,
            Hotel = hotel,
            GuestId = guest.Id,
            Guest = guest,
            ConfirmationNumber = "BK123",
            CheckInDate = DateTime.UtcNow.Date,
            CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
            TotalAmount = 300,
            Status = BookingStatus.Confirmed
        };

        _context.Brands.Add(brand);
        _context.Hotels.Add(hotel);
        _context.Rooms.AddRange(room1, room2);
        _context.Users.Add(guest);
        _context.Bookings.Add(booking);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetBookingStatsAsync_ReturnsCorrectStats()
    {
        // Act
        var result = await _sut.GetBookingStatsAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalBookings);
        Assert.Equal(1, result.ConfirmedBookings);
        Assert.Equal(0, result.PendingBookings);
        Assert.Equal(0, result.CancelledBookings);
    }

    [Fact]
    public async Task GetOccupancyStatsAsync_ReturnsCorrectStats()
    {
        // Act
        var result = await _sut.GetOccupancyStatsAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRooms);
        Assert.Equal(1, result.OccupiedRooms);
        Assert.Equal(1, result.AvailableRooms);
        Assert.Equal(50, result.OccupancyRate); // 1/2 = 50%
    }

    [Fact]
    public async Task GetHotelDashboardAsync_ReturnsFullSummary()
    {
        // Act
        var result = await _sut.GetHotelDashboardAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Revenue);
        Assert.NotNull(result.Bookings);
        Assert.NotNull(result.Occupancy);
        Assert.NotNull(result.Reviews);
        Assert.NotNull(result.TopRooms);
        Assert.NotNull(result.RecentActivities);
    }

    [Fact]
    public async Task GetRevenueStatsAsync_ReturnsRevenueData()
    {
        // Act
        var result = await _sut.GetRevenueStatsAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public async Task GetReviewStatsAsync_ReturnsReviewData()
    {
        // Act
        var result = await _sut.GetReviewStatsAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalReviews);
        Assert.Equal(0, result.PendingReviews);
    }

    [Fact]
    public async Task GetTopRoomsAsync_ReturnsTopRooms()
    {
        // Act
        var result = await _sut.GetTopRoomsAsync(_hotelId, 5);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<TopRoomDto>>(result);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_ReturnsActivities()
    {
        // Act
        var result = await _sut.GetRecentActivitiesAsync(_hotelId, 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Equal("Booking", result.First().Type);
    }

    [Fact]
    public async Task GetRevenueChartAsync_ReturnsChartData()
    {
        // Act
        var result = await _sut.GetRevenueChartAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Daily.Count); // Last 7 days
        Assert.Equal(12, result.Monthly.Count); // Last 12 months
    }

    [Fact]
    public async Task GetBookingChartAsync_ReturnsChartData()
    {
        // Act
        var result = await _sut.GetBookingChartAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ByStatus.Count > 0);
        Assert.Equal(7, result.Daily.Count); // Last 7 days
    }

    [Fact]
    public async Task GetBrandDashboardAsync_ReturnsAggregatedData()
    {
        // Act
        var result = await _sut.GetBrandDashboardAsync(_brandId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Revenue);
        Assert.NotNull(result.Bookings);
        Assert.NotNull(result.Occupancy);
        Assert.Equal(2, result.Occupancy.TotalRooms);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
