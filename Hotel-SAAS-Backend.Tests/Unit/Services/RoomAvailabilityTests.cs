using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class RoomAvailabilityTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly RoomService _sut;

    public RoomAvailabilityTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _sut = new RoomService(
            _roomRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _bookingRepositoryMock.Object);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_WhenHotelNotFound_ReturnsNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3)
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_WithAvailableRooms_ReturnsAvailability()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel" };
        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                RoomNumber = "101",
                BasePrice = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Type = RoomType.Standard,
                BedType = BedType.Queen,
                Images = new List<RoomImage>(),
                Amenities = new List<RoomAmenity>()
            }
        };

        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3),
            NumberOfGuests = 2
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _bookingRepositoryMock.Setup(x => x.GetBookedRoomIdsAsync(hotelId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Guid>());
        _roomRepositoryMock.Setup(x => x.GetAvailableRoomsByHotelAsync(hotelId, It.IsAny<List<Guid>>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(hotelId, result.HotelId);
        Assert.Single(result.AvailableRooms);
        Assert.Equal(200, result.AvailableRooms.First().TotalPrice); // 100 * 2 nights
    }

    [Fact]
    public async Task CheckAvailabilityAsync_WithInvalidDates_ReturnsNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel" };
        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.AddDays(3),
            CheckOutDate = DateTime.UtcNow.AddDays(1) // Invalid: checkout before checkin
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_FiltersRoomsByGuestCapacity()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel" };
        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                BasePrice = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Type = RoomType.Standard,
                BedType = BedType.Queen,
                Images = new List<RoomImage>(),
                Amenities = new List<RoomAmenity>()
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "102",
                BasePrice = 150,
                MaxOccupancy = 4,
                Status = RoomStatus.Available,
                Type = RoomType.Deluxe,
                BedType = BedType.King,
                Images = new List<RoomImage>(),
                Amenities = new List<RoomAmenity>()
            }
        };

        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(2),
            NumberOfGuests = 3 // Should filter out room with MaxOccupancy = 2
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _bookingRepositoryMock.Setup(x => x.GetBookedRoomIdsAsync(hotelId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Guid>());
        _roomRepositoryMock.Setup(x => x.GetAvailableRoomsByHotelAsync(hotelId, It.IsAny<List<Guid>>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.AvailableRooms);
        Assert.Equal("102", result.AvailableRooms.First().RoomNumber);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_ExcludesBookedRooms()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var bookedRoomId = Guid.NewGuid();
        var availableRoomId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel" };

        var availableRooms = new List<Room>
        {
            new Room
            {
                Id = availableRoomId,
                RoomNumber = "102",
                BasePrice = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Type = RoomType.Standard,
                BedType = BedType.Queen,
                Images = new List<RoomImage>(),
                Amenities = new List<RoomAmenity>()
            }
        };

        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3),
            NumberOfGuests = 1
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _bookingRepositoryMock.Setup(x => x.GetBookedRoomIdsAsync(hotelId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Guid> { bookedRoomId });
        _roomRepositoryMock.Setup(x => x.GetAvailableRoomsByHotelAsync(hotelId, It.Is<List<Guid>>(l => l.Contains(bookedRoomId))))
            .ReturnsAsync(availableRooms);

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.AvailableRooms);
        Assert.Equal(availableRoomId, result.AvailableRooms.First().RoomId);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_CalculatesCorrectTotalPrice()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel" };
        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                BasePrice = 75.50m,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Type = RoomType.Standard,
                BedType = BedType.Queen,
                Images = new List<RoomImage>(),
                Amenities = new List<RoomAmenity>()
            }
        };

        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.Date.AddDays(1),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(6), // 5 nights
            NumberOfGuests = 1
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _bookingRepositoryMock.Setup(x => x.GetBookedRoomIdsAsync(hotelId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Guid>());
        _roomRepositoryMock.Setup(x => x.GetAvailableRoomsByHotelAsync(hotelId, It.IsAny<List<Guid>>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.NumberOfNights);
        Assert.Equal(377.50m, result.AvailableRooms.First().TotalPrice); // 75.50 * 5 = 377.50
        Assert.Equal(377.50m, result.LowestPrice);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_WithNoAvailableRooms_ReturnsEmptyList()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel" };

        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3),
            NumberOfGuests = 1
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _bookingRepositoryMock.Setup(x => x.GetBookedRoomIdsAsync(hotelId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Guid>());
        _roomRepositoryMock.Setup(x => x.GetAvailableRoomsByHotelAsync(hotelId, It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<Room>());

        // Act
        var result = await _sut.CheckAvailabilityAsync(hotelId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.AvailableRooms);
        Assert.Equal(0, result.TotalAvailableRooms);
        Assert.Null(result.LowestPrice);
    }
}
