# Phase 6: Guest Profile Enhancement

## ?? M?c tiêu
Nâng cao guest experience v?i booking history filters và recently viewed hotels.

## ?? Tasks

### Task 6.1: Create Recently Viewed Entity

#### File: `Models/Entities/RecentlyViewedHotel.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class RecentlyViewedHotel : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime ViewedAt { get; set; }
        public int ViewCount { get; set; } = 1;
        
        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}
```

---

### Task 6.2: Update DbContext

#### File: `Data/ApplicationDbContext.cs`
```csharp
// Thêm DbSet
public DbSet<RecentlyViewedHotel> RecentlyViewedHotels { get; set; }

// Trong OnModelCreating:
modelBuilder.Entity<RecentlyViewedHotel>(entity =>
{
    entity.ToTable("recently_viewed_hotels");
    entity.HasKey(e => e.Id);
    entity.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();
    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.ViewedAt);

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Hotel)
        .WithMany()
        .HasForeignKey(e => e.HotelId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

---

### Task 6.3: Create DTOs

#### File: `Models/DTOs/GuestProfileDto.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class BookingHistoryFilterDto : PaginationRequestDto
    {
        public BookingStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? HotelId { get; set; }
    }

    public class UpdateGuestPreferencesDto
    {
        public string? PreferredLanguage { get; set; }
        public string? PreferredCurrency { get; set; }
        public bool? EmailNotificationsEnabled { get; set; }
        public bool? SmsNotificationsEnabled { get; set; }
        public string? Nationality { get; set; }
        public string? IdDocumentType { get; set; }
        public string? IdDocumentNumber { get; set; }
    }

    // Response DTOs
    public class GuestProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Nationality { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PreferredCurrency { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool SmsNotificationsEnabled { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Stats
        public int TotalBookings { get; set; }
        public int CompletedStays { get; set; }
        public int TotalReviews { get; set; }
    }

    public class RecentlyViewedHotelDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string? HotelImageUrl { get; set; }
        public string? City { get; set; }
        public int StarRating { get; set; }
        public float? AverageRating { get; set; }
        public decimal? MinPrice { get; set; }
        public DateTime ViewedAt { get; set; }
        public int ViewCount { get; set; }
    }

    public class GuestBookingHistoryDto
    {
        public PagedResultDto<BookingDto> Bookings { get; set; } = new();
        public BookingStatsSummaryDto Stats { get; set; } = new();
    }

    public class BookingStatsSummaryDto
    {
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public string? MostVisitedCity { get; set; }
    }
}
```

---

### Task 6.4: Add Mapper Methods

#### File: `Mapping/Mapper.cs`
```csharp
#region Guest Profile Mappings

public static GuestProfileDto ToGuestProfileDto(User entity, int totalBookings, int completedStays, int totalReviews)
{
    return new GuestProfileDto
    {
        Id = entity.Id,
        Email = entity.Email,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        PhoneNumber = entity.PhoneNumber,
        AvatarUrl = entity.AvatarUrl,
        Nationality = entity.Nationality,
        PreferredLanguage = entity.PreferredLanguage,
        PreferredCurrency = entity.PreferredCurrency,
        EmailNotificationsEnabled = entity.EmailNotificationsEnabled,
        SmsNotificationsEnabled = entity.SmsNotificationsEnabled,
        DateOfBirth = entity.DateOfBirth,
        Address = entity.Address,
        City = entity.City,
        Country = entity.Country,
        CreatedAt = entity.CreatedAt,
        LastLoginAt = entity.LastLoginAt,
        TotalBookings = totalBookings,
        CompletedStays = completedStays,
        TotalReviews = totalReviews
    };
}

public static RecentlyViewedHotelDto ToDto(RecentlyViewedHotel entity)
{
    return new RecentlyViewedHotelDto
    {
        HotelId = entity.HotelId,
        HotelName = entity.Hotel?.Name ?? "",
        HotelImageUrl = entity.Hotel?.ImageUrl,
        City = entity.Hotel?.City,
        StarRating = entity.Hotel?.StarRating ?? 0,
        AverageRating = entity.Hotel?.AverageRating,
        MinPrice = entity.Hotel?.Rooms?.Any() == true 
            ? entity.Hotel.Rooms.Min(r => r.BasePrice) 
            : null,
        ViewedAt = entity.ViewedAt,
        ViewCount = entity.ViewCount
    };
}

public static void UpdateGuestPreferences(UpdateGuestPreferencesDto dto, User entity)
{
    if (dto.PreferredLanguage != null) entity.PreferredLanguage = dto.PreferredLanguage;
    if (dto.PreferredCurrency != null) entity.PreferredCurrency = dto.PreferredCurrency;
    if (dto.EmailNotificationsEnabled.HasValue) entity.EmailNotificationsEnabled = dto.EmailNotificationsEnabled.Value;
    if (dto.SmsNotificationsEnabled.HasValue) entity.SmsNotificationsEnabled = dto.SmsNotificationsEnabled.Value;
    if (dto.Nationality != null) entity.Nationality = dto.Nationality;
    if (dto.IdDocumentType != null) entity.IdDocumentType = dto.IdDocumentType;
    if (dto.IdDocumentNumber != null) entity.IdDocumentNumber = dto.IdDocumentNumber;
    entity.UpdatedAt = DateTime.UtcNow;
}

#endregion
```

---

### Task 6.5: Create Repository

#### File: `Interfaces/Repositories/IRecentlyViewedRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IRecentlyViewedRepository : IBaseRepository<RecentlyViewedHotel>
    {
        Task<IEnumerable<RecentlyViewedHotel>> GetByUserIdAsync(Guid userId, int limit = 10);
        Task<RecentlyViewedHotel?> GetByUserAndHotelAsync(Guid userId, Guid hotelId);
        Task AddOrUpdateViewAsync(Guid userId, Guid hotelId);
        Task ClearUserHistoryAsync(Guid userId);
    }
}
```

#### File: `Repositories/RecentlyViewedRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class RecentlyViewedRepository(ApplicationDbContext context) 
        : BaseRepository<RecentlyViewedHotel>(context), IRecentlyViewedRepository
    {
        public async Task<IEnumerable<RecentlyViewedHotel>> GetByUserIdAsync(Guid userId, int limit = 10)
        {
            return await _dbSet
                .Include(rv => rv.Hotel)
                    .ThenInclude(h => h.Brand)
                .Include(rv => rv.Hotel)
                    .ThenInclude(h => h.Rooms)
                .AsNoTracking()
                .Where(rv => rv.UserId == userId && !rv.IsDeleted && !rv.Hotel.IsDeleted)
                .OrderByDescending(rv => rv.ViewedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<RecentlyViewedHotel?> GetByUserAndHotelAsync(Guid userId, Guid hotelId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(rv => rv.UserId == userId && rv.HotelId == hotelId && !rv.IsDeleted);
        }

        public async Task AddOrUpdateViewAsync(Guid userId, Guid hotelId)
        {
            var existing = await GetByUserAndHotelAsync(userId, hotelId);

            if (existing != null)
            {
                existing.ViewedAt = DateTime.UtcNow;
                existing.ViewCount++;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            else
            {
                var newView = new RecentlyViewedHotel
                {
                    UserId = userId,
                    HotelId = hotelId,
                    ViewedAt = DateTime.UtcNow,
                    ViewCount = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _dbSet.AddAsync(newView);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearUserHistoryAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            await _dbSet
                .Where(rv => rv.UserId == userId && !rv.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rv => rv.IsDeleted, true)
                    .SetProperty(rv => rv.UpdatedAt, now));
        }
    }
}
```

---

### Task 6.6: Create Service

#### File: `Interfaces/Services/IGuestProfileService.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IGuestProfileService
    {
        Task<GuestProfileDto?> GetProfileAsync(Guid userId);
        Task<GuestProfileDto?> UpdatePreferencesAsync(Guid userId, UpdateGuestPreferencesDto dto);
        Task<GuestBookingHistoryDto> GetBookingHistoryAsync(Guid userId, BookingHistoryFilterDto filter);
        Task<IEnumerable<RecentlyViewedHotelDto>> GetRecentlyViewedAsync(Guid userId, int limit = 10);
        Task TrackHotelViewAsync(Guid userId, Guid hotelId);
        Task ClearRecentlyViewedAsync(Guid userId);
    }
}
```

#### File: `Services/GuestProfileService.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services
{
    public class GuestProfileService(
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IRecentlyViewedRepository recentlyViewedRepository,
        ApplicationDbContext context) : IGuestProfileService
    {
        public async Task<GuestProfileDto?> GetProfileAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var totalBookings = await context.Bookings
                .CountAsync(b => b.GuestId == userId && !b.IsDeleted);

            var completedStays = await context.Bookings
                .CountAsync(b => b.GuestId == userId && 
                                !b.IsDeleted && 
                                b.Status == BookingStatus.CheckedOut);

            var totalReviews = await context.Reviews
                .CountAsync(r => r.GuestId == userId && !r.IsDeleted);

            return Mapper.ToGuestProfileDto(user, totalBookings, completedStays, totalReviews);
        }

        public async Task<GuestProfileDto?> UpdatePreferencesAsync(Guid userId, UpdateGuestPreferencesDto dto)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            Mapper.UpdateGuestPreferences(dto, user);
            await userRepository.UpdateAsync(user);

            return await GetProfileAsync(userId);
        }

        public async Task<GuestBookingHistoryDto> GetBookingHistoryAsync(Guid userId, BookingHistoryFilterDto filter)
        {
            var query = context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .Where(b => b.GuestId == userId && !b.IsDeleted);

            // Apply filters
            if (filter.Status.HasValue)
                query = query.Where(b => b.Status == filter.Status.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(b => b.CheckInDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(b => b.CheckOutDate <= filter.ToDate.Value);

            if (filter.HotelId.HasValue)
                query = query.Where(b => b.HotelId == filter.HotelId.Value);

            var totalCount = await query.CountAsync();

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "date" => filter.SortDescending 
                    ? query.OrderByDescending(b => b.CheckInDate)
                    : query.OrderBy(b => b.CheckInDate),
                "amount" => filter.SortDescending
                    ? query.OrderByDescending(b => b.TotalAmount)
                    : query.OrderBy(b => b.TotalAmount),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            // Pagination
            var bookings = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var items = bookings.Select(Mapper.ToDto).ToList();

            // Calculate stats
            var allBookings = await context.Bookings
                .Include(b => b.Hotel)
                .Where(b => b.GuestId == userId && !b.IsDeleted)
                .ToListAsync();

            var stats = new BookingStatsSummaryDto
            {
                TotalBookings = allBookings.Count,
                CompletedBookings = allBookings.Count(b => b.Status == BookingStatus.CheckedOut),
                CancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled),
                UpcomingBookings = allBookings.Count(b => 
                    (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                    b.CheckInDate > DateTime.UtcNow),
                TotalSpent = allBookings
                    .Where(b => b.Status == BookingStatus.CheckedOut)
                    .Sum(b => b.TotalAmount),
                MostVisitedCity = allBookings
                    .Where(b => b.Hotel?.City != null)
                    .GroupBy(b => b.Hotel!.City)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key
            };

            return new GuestBookingHistoryDto
            {
                Bookings = new PagedResultDto<BookingDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                },
                Stats = stats
            };
        }

        public async Task<IEnumerable<RecentlyViewedHotelDto>> GetRecentlyViewedAsync(Guid userId, int limit = 10)
        {
            var recentlyViewed = await recentlyViewedRepository.GetByUserIdAsync(userId, limit);
            return recentlyViewed.Select(Mapper.ToDto);
        }

        public async Task TrackHotelViewAsync(Guid userId, Guid hotelId)
        {
            await recentlyViewedRepository.AddOrUpdateViewAsync(userId, hotelId);
        }

        public async Task ClearRecentlyViewedAsync(Guid userId)
        {
            await recentlyViewedRepository.ClearUserHistoryAsync(userId);
        }
    }
}
```

---

### Task 6.7: Create Controller

#### File: `Controllers/GuestProfileController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/guest")]
    [Authorize]
    public class GuestProfileController : ControllerBase
    {
        private readonly IGuestProfileService _guestProfileService;

        public GuestProfileController(IGuestProfileService guestProfileService)
        {
            _guestProfileService = guestProfileService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDto<GuestProfileDto>>> GetProfile()
        {
            var result = await _guestProfileService.GetProfileAsync(GetUserId());
            if (result == null)
            {
                return NotFound(new ApiResponseDto<GuestProfileDto>
                {
                    Success = false,
                    Message = "Profile not found"
                });
            }

            return Ok(new ApiResponseDto<GuestProfileDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPut("preferences")]
        public async Task<ActionResult<ApiResponseDto<GuestProfileDto>>> UpdatePreferences(
            [FromBody] UpdateGuestPreferencesDto dto)
        {
            var result = await _guestProfileService.UpdatePreferencesAsync(GetUserId(), dto);
            if (result == null)
            {
                return NotFound(new ApiResponseDto<GuestProfileDto>
                {
                    Success = false,
                    Message = "Profile not found"
                });
            }

            return Ok(new ApiResponseDto<GuestProfileDto>
            {
                Success = true,
                Message = "Preferences updated",
                Data = result
            });
        }

        [HttpGet("bookings")]
        public async Task<ActionResult<ApiResponseDto<GuestBookingHistoryDto>>> GetBookingHistory(
            [FromQuery] BookingHistoryFilterDto filter)
        {
            var result = await _guestProfileService.GetBookingHistoryAsync(GetUserId(), filter);
            return Ok(new ApiResponseDto<GuestBookingHistoryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("recently-viewed")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<RecentlyViewedHotelDto>>>> GetRecentlyViewed(
            [FromQuery] int limit = 10)
        {
            var result = await _guestProfileService.GetRecentlyViewedAsync(GetUserId(), limit);
            return Ok(new ApiResponseDto<IEnumerable<RecentlyViewedHotelDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpDelete("recently-viewed")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ClearRecentlyViewed()
        {
            await _guestProfileService.ClearRecentlyViewedAsync(GetUserId());
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "History cleared",
                Data = true
            });
        }
    }
}
```

---

### Task 6.8: Integrate Hotel View Tracking

#### File: `Controllers/HotelsController.cs` - Update GetHotelDetails
```csharp
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IRoomService _roomService;
    private readonly IGuestProfileService _guestProfileService;  // Thêm dependency

    public HotelsController(
        IHotelService hotelService, 
        IRoomService roomService,
        IGuestProfileService guestProfileService)
    {
        _hotelService = hotelService;
        _roomService = roomService;
        _guestProfileService = guestProfileService;
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<ApiResponseDto<HotelDetailDto>>> GetHotelDetails(Guid id)
    {
        var hotel = await _hotelService.GetHotelDetailByIdAsync(id);
        if (hotel == null)
        {
            return NotFound(new ApiResponseDto<HotelDetailDto>
            {
                Success = false,
                Message = "Hotel not found"
            });
        }

        // Track view if user is authenticated
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _ = _guestProfileService.TrackHotelViewAsync(userId, id);  // Fire and forget
        }

        return Ok(new ApiResponseDto<HotelDetailDto>
        {
            Success = true,
            Data = hotel
        });
    }
}
```

---

### Task 6.9: Register in Program.cs

```csharp
// Repository
builder.Services.AddScoped<IRecentlyViewedRepository, RecentlyViewedRepository>();

// Service
builder.Services.AddScoped<IGuestProfileService, GuestProfileService>();
```

---

### Task 6.10: Create Migration

```bash
dotnet ef migrations add AddRecentlyViewedHotels
dotnet ef database update
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/GuestProfileServiceTests.cs`

```csharp
using Xunit;
using Moq;
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
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IRecentlyViewedRepository> _recentlyViewedRepoMock;
    private readonly GuestProfileService _sut;
    private readonly Guid _userId;

    public GuestProfileServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _userRepoMock = new Mock<IUserRepository>();
        _bookingRepoMock = new Mock<IBookingRepository>();
        _recentlyViewedRepoMock = new Mock<IRecentlyViewedRepository>();

        _sut = new GuestProfileService(
            _userRepoMock.Object,
            _bookingRepoMock.Object,
            _recentlyViewedRepoMock.Object,
            _context);

        _userId = Guid.NewGuid();
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
            PasswordHash = "hash"
        };

        var brand = new Brand { Id = Guid.NewGuid(), Name = "Test Brand" };
        var hotel = new Hotel { Id = Guid.NewGuid(), Name = "Test Hotel", BrandId = brand.Id, Brand = brand };

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
            PreferredLanguage = "de"
        };

        // Act
        var result = await _sut.UpdatePreferencesAsync(_userId, dto);

        // Assert
        Assert.NotNull(result);
        _userRepoMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

---

## ? Checklist

- [ ] Entity `RecentlyViewedHotel` ?ã t?o
- [ ] DbContext ?ã update
- [ ] DTOs ?ã t?o (`GuestProfileDto.cs`)
- [ ] Mapper methods ?ã thêm
- [ ] `IRecentlyViewedRepository` ?ã t?o
- [ ] `RecentlyViewedRepository` ?ã implement
- [ ] `IGuestProfileService` ?ã t?o
- [ ] `GuestProfileService` ?ã implement
- [ ] `GuestProfileController` ?ã t?o
- [ ] `HotelsController` ?ã update ?? track views
- [ ] Registered trong `Program.cs`
- [ ] Migration ?ã t?o
- [ ] Unit tests ?ã vi?t
- [ ] Build passes

---

## ?? API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/guest/profile` | User | Get guest profile with stats |
| PUT | `/api/guest/preferences` | User | Update preferences |
| GET | `/api/guest/bookings` | User | Booking history with filters |
| GET | `/api/guest/recently-viewed` | User | Recently viewed hotels |
| DELETE | `/api/guest/recently-viewed` | User | Clear view history |
