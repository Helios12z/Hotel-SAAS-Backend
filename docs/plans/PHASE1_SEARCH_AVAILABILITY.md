# Phase 1: Search & Availability

## ?? M?c tiêu
Cho phép user search hotels v?i filters nâng cao và check room availability theo ngày.

## ?? Tasks

### Task 1.1: Pagination Support
**Files c?n t?o/s?a:**

#### 1. `Models/DTOs/CommonDto.cs` - Thêm pagination DTOs
```csharp
// Thêm vào file hi?n có

public class PaginationRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```

---

### Task 1.2: Advanced Hotel Search
**Files c?n t?o/s?a:**

#### 1. `Models/DTOs/HotelDto.cs` - Thêm search request DTO
```csharp
// Thêm vào file hi?n có

public class HotelSearchRequestDto : PaginationRequestDto
{
    public string? Query { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public int? MinStarRating { get; set; }
    public int? MaxStarRating { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? NumberOfGuests { get; set; }
    public int? NumberOfRooms { get; set; }
    public List<Guid>? AmenityIds { get; set; }
    public float? MinRating { get; set; }
}

public class HotelSearchResultDto : HotelDto
{
    public int AvailableRooms { get; set; }
    public decimal? LowestAvailablePrice { get; set; }
}
```

#### 2. `Interfaces/Repositories/IHotelRepository.cs` - Thêm search method
```csharp
// Thêm vào interface hi?n có

Task<(IEnumerable<Hotel> Hotels, int TotalCount)> SearchWithPaginationAsync(
    string? query,
    string? city,
    string? country,
    int? minStarRating,
    int? maxStarRating,
    decimal? minPrice,
    decimal? maxPrice,
    List<Guid>? amenityIds,
    float? minRating,
    int page,
    int pageSize,
    string? sortBy,
    bool sortDescending);
```

#### 3. `Repositories/HotelRepository.cs` - Implement search
```csharp
// Thêm method vào class hi?n có

public async Task<(IEnumerable<Hotel> Hotels, int TotalCount)> SearchWithPaginationAsync(
    string? query,
    string? city,
    string? country,
    int? minStarRating,
    int? maxStarRating,
    decimal? minPrice,
    decimal? maxPrice,
    List<Guid>? amenityIds,
    float? minRating,
    int page,
    int pageSize,
    string? sortBy,
    bool sortDescending)
{
    var hotels = _dbSet
        .Include(h => h.Brand)
        .Include(h => h.Rooms)
        .Include(h => h.Amenities)
        .AsNoTracking()
        .Where(h => !h.IsDeleted && h.IsActive);

    // Apply filters
    if (!string.IsNullOrWhiteSpace(query))
    {
        var lowerQuery = query.ToLower();
        hotels = hotels.Where(h =>
            h.Name.ToLower().Contains(lowerQuery) ||
            (h.Description != null && h.Description.ToLower().Contains(lowerQuery)) ||
            (h.City != null && h.City.ToLower().Contains(lowerQuery)));
    }

    if (!string.IsNullOrWhiteSpace(city))
        hotels = hotels.Where(h => h.City != null && h.City.ToLower() == city.ToLower());

    if (!string.IsNullOrWhiteSpace(country))
        hotels = hotels.Where(h => h.Country != null && h.Country.ToLower() == country.ToLower());

    if (minStarRating.HasValue)
        hotels = hotels.Where(h => h.StarRating >= minStarRating.Value);

    if (maxStarRating.HasValue)
        hotels = hotels.Where(h => h.StarRating <= maxStarRating.Value);

    if (minPrice.HasValue)
        hotels = hotels.Where(h => h.Rooms.Any(r => r.BasePrice >= minPrice.Value));

    if (maxPrice.HasValue)
        hotels = hotels.Where(h => h.Rooms.Any(r => r.BasePrice <= maxPrice.Value));

    if (minRating.HasValue)
        hotels = hotels.Where(h => h.AverageRating >= minRating.Value);

    if (amenityIds != null && amenityIds.Count > 0)
        hotels = hotels.Where(h => h.Amenities.Any(a => amenityIds.Contains(a.AmenityId)));

    // Get total count before pagination
    var totalCount = await hotels.CountAsync();

    // Apply sorting
    hotels = sortBy?.ToLower() switch
    {
        "price" => sortDescending 
            ? hotels.OrderByDescending(h => h.Rooms.Min(r => r.BasePrice))
            : hotels.OrderBy(h => h.Rooms.Min(r => r.BasePrice)),
        "rating" => sortDescending
            ? hotels.OrderByDescending(h => h.AverageRating)
            : hotels.OrderBy(h => h.AverageRating),
        "name" => sortDescending
            ? hotels.OrderByDescending(h => h.Name)
            : hotels.OrderBy(h => h.Name),
        "star" => sortDescending
            ? hotels.OrderByDescending(h => h.StarRating)
            : hotels.OrderBy(h => h.StarRating),
        _ => hotels.OrderByDescending(h => h.AverageRating) // Default sort
    };

    // Apply pagination
    var result = await hotels
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (result, totalCount);
}
```

#### 4. `Interfaces/Services/IHotelService.cs` - Thêm search method
```csharp
// Thêm vào interface hi?n có

Task<PagedResultDto<HotelSearchResultDto>> SearchHotelsAsync(HotelSearchRequestDto request);
```

#### 5. `Services/HotelService.cs` - Implement advanced search
```csharp
// Thêm method vào class hi?n có

public async Task<PagedResultDto<HotelSearchResultDto>> SearchHotelsAsync(HotelSearchRequestDto request)
{
    var (hotels, totalCount) = await _hotelRepository.SearchWithPaginationAsync(
        request.Query,
        request.City,
        request.Country,
        request.MinStarRating,
        request.MaxStarRating,
        request.MinPrice,
        request.MaxPrice,
        request.AmenityIds,
        request.MinRating,
        request.Page,
        request.PageSize,
        request.SortBy,
        request.SortDescending);

    var items = hotels.Select(h => new HotelSearchResultDto
    {
        Id = h.Id,
        CreatedAt = h.CreatedAt,
        UpdatedAt = h.UpdatedAt,
        BrandId = h.BrandId,
        BrandName = h.Brand?.Name ?? "",
        Name = h.Name,
        Description = h.Description,
        ImageUrl = h.ImageUrl,
        City = h.City,
        Country = h.Country,
        StarRating = h.StarRating,
        IsActive = h.IsActive,
        IsVerified = h.IsVerified,
        AverageRating = h.AverageRating,
        ReviewCount = h.ReviewCount,
        MinPrice = h.Rooms?.Any() == true ? h.Rooms.Min(r => r.BasePrice) : null,
        AvailableRooms = h.Rooms?.Count(r => r.Status == RoomStatus.Available) ?? 0,
        LowestAvailablePrice = h.Rooms?
            .Where(r => r.Status == RoomStatus.Available)
            .OrderBy(r => r.BasePrice)
            .FirstOrDefault()?.BasePrice
    }).ToList();

    return new PagedResultDto<HotelSearchResultDto>
    {
        Items = items,
        TotalCount = totalCount,
        Page = request.Page,
        PageSize = request.PageSize
    };
}
```

#### 6. `Controllers/HotelsController.cs` - Update search endpoint
```csharp
// Thay th? method SearchHotels hi?n có

[HttpGet("search")]
public async Task<ActionResult<ApiResponseDto<PagedResultDto<HotelSearchResultDto>>>> SearchHotels(
    [FromQuery] HotelSearchRequestDto request)
{
    var result = await _hotelService.SearchHotelsAsync(request);
    return Ok(new ApiResponseDto<PagedResultDto<HotelSearchResultDto>>
    {
        Success = true,
        Data = result
    });
}
```

---

### Task 1.3: Room Availability Check
**Files c?n t?o/s?a:**

#### 1. `Models/DTOs/RoomDto.cs` - Thêm availability DTOs
```csharp
// Thêm vào file hi?n có

public class RoomAvailabilityRequestDto
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; } = 1;
}

public class RoomAvailabilityDto
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public BedType BedType { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int NumberOfNights { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public List<AmenityDto> Amenities { get; set; } = new();
}

public class HotelAvailabilityDto
{
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public List<RoomAvailabilityDto> AvailableRooms { get; set; } = new();
    public int TotalAvailableRooms { get; set; }
    public decimal? LowestPrice { get; set; }
}
```

#### 2. `Interfaces/Repositories/IBookingRepository.cs` - Thêm availability check
```csharp
// Thêm vào interface hi?n có

Task<List<Guid>> GetBookedRoomIdsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut);
```

#### 3. `Repositories/BookingRepository.cs` - Implement availability check
```csharp
// Thêm method vào class hi?n có

public async Task<List<Guid>> GetBookedRoomIdsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut)
{
    // L?y danh sách room IDs ?ã ???c book trong kho?ng th?i gian
    var bookedRoomIds = await _context.BookingRooms
        .Include(br => br.Booking)
        .Where(br => 
            br.Booking.HotelId == hotelId &&
            br.Booking.Status != BookingStatus.Cancelled &&
            br.Booking.Status != BookingStatus.CheckedOut &&
            // Check date overlap
            br.Booking.CheckInDate < checkOut &&
            br.Booking.CheckOutDate > checkIn)
        .Select(br => br.RoomId)
        .Distinct()
        .ToListAsync();

    return bookedRoomIds;
}
```

#### 4. `Interfaces/Repositories/IRoomRepository.cs` - Thêm methods
```csharp
// Thêm vào interface hi?n có

Task<IEnumerable<Room>> GetAvailableRoomsByHotelAsync(Guid hotelId, List<Guid> excludeRoomIds);
Task<Room?> GetByIdWithDetailsAsync(Guid id);
```

#### 5. `Repositories/RoomRepository.cs` - Implement methods
```csharp
// Thêm methods vào class hi?n có

public async Task<IEnumerable<Room>> GetAvailableRoomsByHotelAsync(Guid hotelId, List<Guid> excludeRoomIds)
{
    return await _dbSet
        .Include(r => r.Hotel)
        .Include(r => r.Images)
        .Include(r => r.Amenities)
            .ThenInclude(ra => ra.Amenity)
        .AsNoTracking()
        .Where(r => 
            r.HotelId == hotelId && 
            !r.IsDeleted &&
            r.Status == RoomStatus.Available &&
            !excludeRoomIds.Contains(r.Id))
        .OrderBy(r => r.BasePrice)
        .ToListAsync();
}

public async Task<Room?> GetByIdWithDetailsAsync(Guid id)
{
    return await _dbSet
        .Include(r => r.Hotel)
        .Include(r => r.Images)
        .Include(r => r.Amenities)
            .ThenInclude(ra => ra.Amenity)
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
}
```

#### 6. `Interfaces/Services/IRoomService.cs` - Thêm availability method
```csharp
// Thêm vào interface hi?n có

Task<HotelAvailabilityDto?> CheckAvailabilityAsync(Guid hotelId, RoomAvailabilityRequestDto request);
```

#### 7. `Services/RoomService.cs` - Implement availability check
```csharp
// Thêm dependency và method vào class hi?n có

public class RoomService(
    IRoomRepository roomRepository,
    IHotelRepository hotelRepository,
    IBookingRepository bookingRepository) : IRoomService
{
    // ... existing methods ...

    public async Task<HotelAvailabilityDto?> CheckAvailabilityAsync(Guid hotelId, RoomAvailabilityRequestDto request)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId);
        if (hotel == null) return null;

        var numberOfNights = (int)(request.CheckOutDate - request.CheckInDate).TotalDays;
        if (numberOfNights <= 0) return null;

        // Get booked room IDs for the date range
        var bookedRoomIds = await bookingRepository.GetBookedRoomIdsAsync(
            hotelId, request.CheckInDate, request.CheckOutDate);

        // Get available rooms
        var availableRooms = await roomRepository.GetAvailableRoomsByHotelAsync(hotelId, bookedRoomIds);

        // Filter by guest capacity if specified
        if (request.NumberOfGuests > 0)
        {
            availableRooms = availableRooms.Where(r => r.MaxOccupancy >= request.NumberOfGuests);
        }

        var roomDtos = availableRooms.Select(r => new RoomAvailabilityDto
        {
            RoomId = r.Id,
            RoomNumber = r.RoomNumber,
            Type = r.Type,
            BedType = r.BedType,
            MaxOccupancy = r.MaxOccupancy,
            BasePrice = r.BasePrice,
            TotalPrice = r.BasePrice * numberOfNights,
            NumberOfNights = numberOfNights,
            IsAvailable = true,
            ImageUrl = r.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? r.Images?.FirstOrDefault()?.ImageUrl,
            Description = r.Description,
            Amenities = r.Amenities?.Select(ra => Mapper.ToDto(ra.Amenity)).ToList() ?? new()
        }).ToList();

        return new HotelAvailabilityDto
        {
            HotelId = hotelId,
            HotelName = hotel.Name,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            NumberOfNights = numberOfNights,
            AvailableRooms = roomDtos,
            TotalAvailableRooms = roomDtos.Count,
            LowestPrice = roomDtos.Any() ? roomDtos.Min(r => r.TotalPrice) : null
        };
    }
}
```

#### 8. `Controllers/HotelsController.cs` - Thêm availability endpoint
```csharp
// Thêm method m?i và inject RoomService

public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IRoomService _roomService;

    public HotelsController(IHotelService hotelService, IRoomService roomService)
    {
        _hotelService = hotelService;
        _roomService = roomService;
    }

    // ... existing methods ...

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<ApiResponseDto<HotelAvailabilityDto>>> CheckAvailability(
        Guid id,
        [FromQuery] DateTime checkInDate,
        [FromQuery] DateTime checkOutDate,
        [FromQuery] int numberOfGuests = 1)
    {
        if (checkInDate >= checkOutDate)
        {
            return BadRequest(new ApiResponseDto<HotelAvailabilityDto>
            {
                Success = false,
                Message = "Check-out date must be after check-in date"
            });
        }

        if (checkInDate < DateTime.UtcNow.Date)
        {
            return BadRequest(new ApiResponseDto<HotelAvailabilityDto>
            {
                Success = false,
                Message = "Check-in date cannot be in the past"
            });
        }

        var request = new RoomAvailabilityRequestDto
        {
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            NumberOfGuests = numberOfGuests
        };

        var result = await _roomService.CheckAvailabilityAsync(id, request);
        if (result == null)
        {
            return NotFound(new ApiResponseDto<HotelAvailabilityDto>
            {
                Success = false,
                Message = "Hotel not found"
            });
        }

        return Ok(new ApiResponseDto<HotelAvailabilityDto>
        {
            Success = true,
            Data = result
        });
    }
}
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/HotelServiceSearchTests.cs`

```csharp
using Xunit;
using Moq;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class HotelServiceSearchTests
{
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly HotelService _sut;

    public HotelServiceSearchTests()
    {
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _sut = new HotelService(_hotelRepositoryMock.Object);
    }

    [Fact]
    public async Task SearchHotelsAsync_WithValidRequest_ReturnsPagedResult()
    {
        // Arrange
        var request = new HotelSearchRequestDto
        {
            Query = "Hanoi",
            Page = 1,
            PageSize = 10
        };

        var hotels = new List<Hotel>
        {
            new Hotel { Id = Guid.NewGuid(), Name = "Hanoi Hotel", Brand = new Brand { Name = "Test" } }
        };

        _hotelRepositoryMock
            .Setup(x => x.SearchWithPaginationAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<List<Guid>?>(), It.IsAny<float?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .ReturnsAsync((hotels, 1));

        // Act
        var result = await _sut.SearchHotelsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task SearchHotelsAsync_WithNoResults_ReturnsEmptyPagedResult()
    {
        // Arrange
        var request = new HotelSearchRequestDto { Query = "NonExistent", Page = 1, PageSize = 10 };
        
        _hotelRepositoryMock
            .Setup(x => x.SearchWithPaginationAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<List<Guid>?>(), It.IsAny<float?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .ReturnsAsync((new List<Hotel>(), 0));

        // Act
        var result = await _sut.SearchHotelsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}
```

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/RoomAvailabilityTests.cs`

```csharp
using Xunit;
using Moq;
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
                BedType = BedType.Queen
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
}
```

---

## ? Checklist

- [ ] `PaginationRequestDto` và `PagedResultDto<T>` ?ã thêm vào `CommonDto.cs`
- [ ] `HotelSearchRequestDto` và `HotelSearchResultDto` ?ã thêm vào `HotelDto.cs`
- [ ] `IHotelRepository.SearchWithPaginationAsync()` ?ã thêm
- [ ] `HotelRepository.SearchWithPaginationAsync()` ?ã implement
- [ ] `IHotelService.SearchHotelsAsync()` ?ã thêm
- [ ] `HotelService.SearchHotelsAsync()` ?ã implement
- [ ] `HotelsController.SearchHotels()` ?ã update
- [ ] `RoomAvailabilityRequestDto` và related DTOs ?ã thêm
- [ ] `IBookingRepository.GetBookedRoomIdsAsync()` ?ã thêm
- [ ] `BookingRepository.GetBookedRoomIdsAsync()` ?ã implement
- [ ] `IRoomRepository` methods ?ã thêm
- [ ] `RoomRepository` methods ?ã implement
- [ ] `IRoomService.CheckAvailabilityAsync()` ?ã thêm
- [ ] `RoomService.CheckAvailabilityAsync()` ?ã implement
- [ ] `HotelsController` inject `IRoomService`
- [ ] `HotelsController.CheckAvailability()` endpoint ?ã thêm
- [ ] Unit tests ?ã vi?t
- [ ] Build passes
- [ ] Manual test via Swagger

---

## ?? API Endpoints sau khi hoàn thành

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/hotels/search?query=&city=&minPrice=&page=&pageSize=` | Advanced search v?i pagination |
| GET | `/api/hotels/{id}/availability?checkInDate=&checkOutDate=&numberOfGuests=` | Check room availability |
