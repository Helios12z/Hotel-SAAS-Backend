# Phase 3: Wishlist & Favorites

## ?? M?c tiêu
Cho phép user l?u hotels yêu thích ?? xem l?i sau.

## ?? Tasks

### Task 3.1: Create Entity

#### File: `Models/Entities/Wishlist.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public string? Note { get; set; }  // Optional: user's note
        
        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}
```

---

### Task 3.2: Update DbContext

#### File: `Data/ApplicationDbContext.cs`
```csharp
// Thêm DbSet
public DbSet<Wishlist> Wishlists { get; set; }

// Trong OnModelCreating:
modelBuilder.Entity<Wishlist>(entity =>
{
    entity.ToTable("wishlists");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Note).HasMaxLength(500);
    entity.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();
    entity.HasIndex(e => e.UserId);

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

### Task 3.3: Create DTOs

#### File: `Models/DTOs/WishlistDto.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class AddToWishlistDto
    {
        public Guid HotelId { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateWishlistDto
    {
        public string? Note { get; set; }
    }

    // Response DTOs
    public class WishlistDto : BaseDto
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public string? Note { get; set; }
        public HotelDto Hotel { get; set; } = null!;
    }

    public class WishlistSummaryDto
    {
        public int TotalItems { get; set; }
        public List<WishlistDto> Items { get; set; } = new();
    }
}
```

---

### Task 3.4: Add Mapper Methods

#### File: `Mapping/Mapper.cs`
```csharp
#region Wishlist Mappings

public static WishlistDto ToDto(Wishlist entity)
{
    return new WishlistDto
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        UserId = entity.UserId,
        HotelId = entity.HotelId,
        Note = entity.Note,
        Hotel = ToDto(entity.Hotel)
    };
}

public static Wishlist ToEntity(AddToWishlistDto dto, Guid userId)
{
    return new Wishlist
    {
        UserId = userId,
        HotelId = dto.HotelId,
        Note = dto.Note,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}

#endregion
```

---

### Task 3.5: Create Repository

#### File: `Interfaces/Repositories/IWishlistRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IWishlistRepository : IBaseRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId);
        Task<Wishlist?> GetByUserAndHotelAsync(Guid userId, Guid hotelId);
        Task<bool> ExistsAsync(Guid userId, Guid hotelId);
        Task<int> GetCountByUserAsync(Guid userId);
    }
}
```

#### File: `Repositories/WishlistRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class WishlistRepository(ApplicationDbContext context) 
        : BaseRepository<Wishlist>(context), IWishlistRepository
    {
        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Brand)
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Rooms)
                .AsNoTracking()
                .Where(w => w.UserId == userId && !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetByUserAndHotelAsync(Guid userId, Guid hotelId)
        {
            return await _dbSet
                .Include(w => w.Hotel)
                    .ThenInclude(h => h.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.HotelId == hotelId && !w.IsDeleted);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid hotelId)
        {
            return await _dbSet.AnyAsync(w => w.UserId == userId && w.HotelId == hotelId && !w.IsDeleted);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _dbSet.CountAsync(w => w.UserId == userId && !w.IsDeleted);
        }
    }
}
```

---

### Task 3.6: Create Service

#### File: `Interfaces/Services/IWishlistService.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<WishlistSummaryDto> GetUserWishlistAsync(Guid userId);
        Task<WishlistDto?> GetByIdAsync(Guid id);
        Task<WishlistDto> AddToWishlistAsync(Guid userId, AddToWishlistDto dto);
        Task<WishlistDto?> UpdateNoteAsync(Guid userId, Guid hotelId, UpdateWishlistDto dto);
        Task<bool> RemoveFromWishlistAsync(Guid userId, Guid hotelId);
        Task<bool> IsInWishlistAsync(Guid userId, Guid hotelId);
        Task<bool> ToggleWishlistAsync(Guid userId, Guid hotelId);
    }
}
```

#### File: `Services/WishlistService.cs`
```csharp
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Services
{
    public class WishlistService(
        IWishlistRepository wishlistRepository,
        IHotelRepository hotelRepository) : IWishlistService
    {
        public async Task<WishlistSummaryDto> GetUserWishlistAsync(Guid userId)
        {
            var wishlists = await wishlistRepository.GetByUserIdAsync(userId);
            var items = wishlists.Select(Mapper.ToDto).ToList();

            return new WishlistSummaryDto
            {
                TotalItems = items.Count,
                Items = items
            };
        }

        public async Task<WishlistDto?> GetByIdAsync(Guid id)
        {
            var wishlist = await wishlistRepository.GetByIdAsync(id);
            return wishlist == null ? null : Mapper.ToDto(wishlist);
        }

        public async Task<WishlistDto> AddToWishlistAsync(Guid userId, AddToWishlistDto dto)
        {
            // Check if hotel exists
            var hotel = await hotelRepository.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException("Hotel not found");

            // Check if already in wishlist
            if (await wishlistRepository.ExistsAsync(userId, dto.HotelId))
                throw new InvalidOperationException("Hotel already in wishlist");

            var wishlist = Mapper.ToEntity(dto, userId);
            var created = await wishlistRepository.CreateAsync(wishlist);

            // Reload with hotel data
            var result = await wishlistRepository.GetByUserAndHotelAsync(userId, dto.HotelId);
            return Mapper.ToDto(result!);
        }

        public async Task<WishlistDto?> UpdateNoteAsync(Guid userId, Guid hotelId, UpdateWishlistDto dto)
        {
            var wishlist = await wishlistRepository.GetByUserAndHotelAsync(userId, hotelId);
            if (wishlist == null) return null;

            // Need to get the tracked entity for update
            var trackedWishlist = await wishlistRepository.GetByIdAsync(wishlist.Id);
            if (trackedWishlist == null) return null;

            trackedWishlist.Note = dto.Note;
            trackedWishlist.UpdatedAt = DateTime.UtcNow;
            
            await wishlistRepository.UpdateAsync(trackedWishlist);

            var updated = await wishlistRepository.GetByUserAndHotelAsync(userId, hotelId);
            return Mapper.ToDto(updated!);
        }

        public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid hotelId)
        {
            var wishlist = await wishlistRepository.GetByUserAndHotelAsync(userId, hotelId);
            if (wishlist == null) return false;

            return await wishlistRepository.DeleteAsync(wishlist.Id);
        }

        public async Task<bool> IsInWishlistAsync(Guid userId, Guid hotelId)
        {
            return await wishlistRepository.ExistsAsync(userId, hotelId);
        }

        public async Task<bool> ToggleWishlistAsync(Guid userId, Guid hotelId)
        {
            var exists = await wishlistRepository.ExistsAsync(userId, hotelId);
            
            if (exists)
            {
                await RemoveFromWishlistAsync(userId, hotelId);
                return false; // Removed
            }
            else
            {
                await AddToWishlistAsync(userId, new AddToWishlistDto { HotelId = hotelId });
                return true; // Added
            }
        }
    }
}
```

---

### Task 3.7: Create Controller

#### File: `Controllers/WishlistController.cs`
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
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<WishlistSummaryDto>>> GetMyWishlist()
        {
            var result = await _wishlistService.GetUserWishlistAsync(GetUserId());
            return Ok(new ApiResponseDto<WishlistSummaryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("check/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> CheckInWishlist(Guid hotelId)
        {
            var result = await _wishlistService.IsInWishlistAsync(GetUserId(), hotelId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<WishlistDto>>> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            try
            {
                var result = await _wishlistService.AddToWishlistAsync(GetUserId(), dto);
                return Ok(new ApiResponseDto<WishlistDto>
                {
                    Success = true,
                    Message = "Added to wishlist",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto<WishlistDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseDto<WishlistDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("toggle/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ToggleWishlist(Guid hotelId)
        {
            try
            {
                var isAdded = await _wishlistService.ToggleWishlistAsync(GetUserId(), hotelId);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = isAdded ? "Added to wishlist" : "Removed from wishlist",
                    Data = isAdded
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Hotel not found"
                });
            }
        }

        [HttpPut("{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<WishlistDto>>> UpdateNote(
            Guid hotelId, 
            [FromBody] UpdateWishlistDto dto)
        {
            var result = await _wishlistService.UpdateNoteAsync(GetUserId(), hotelId, dto);
            if (result == null)
            {
                return NotFound(new ApiResponseDto<WishlistDto>
                {
                    Success = false,
                    Message = "Item not found in wishlist"
                });
            }

            return Ok(new ApiResponseDto<WishlistDto>
            {
                Success = true,
                Message = "Note updated",
                Data = result
            });
        }

        [HttpDelete("{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> RemoveFromWishlist(Guid hotelId)
        {
            var result = await _wishlistService.RemoveFromWishlistAsync(GetUserId(), hotelId);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Removed from wishlist" : "Item not found in wishlist",
                Data = result
            });
        }
    }
}
```

---

### Task 3.8: Register in Program.cs

```csharp
// Repository
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

// Service
builder.Services.AddScoped<IWishlistService, WishlistService>();
```

---

### Task 3.9: Create Migration

```bash
dotnet ef migrations add AddWishlist
dotnet ef database update
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/WishlistServiceTests.cs`

```csharp
using Xunit;
using Moq;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class WishlistServiceTests
{
    private readonly Mock<IWishlistRepository> _wishlistRepoMock;
    private readonly Mock<IHotelRepository> _hotelRepoMock;
    private readonly WishlistService _sut;

    public WishlistServiceTests()
    {
        _wishlistRepoMock = new Mock<IWishlistRepository>();
        _hotelRepoMock = new Mock<IHotelRepository>();
        _sut = new WishlistService(_wishlistRepoMock.Object, _hotelRepoMock.Object);
    }

    [Fact]
    public async Task GetUserWishlistAsync_ReturnsWishlistSummary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotel = new Hotel { Id = Guid.NewGuid(), Name = "Test Hotel", Brand = new Brand { Name = "Brand" } };
        var wishlists = new List<Wishlist>
        {
            new Wishlist { Id = Guid.NewGuid(), UserId = userId, HotelId = hotel.Id, Hotel = hotel }
        };
        _wishlistRepoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(wishlists);

        // Act
        var result = await _sut.GetUserWishlistAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalItems);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task AddToWishlistAsync_WhenHotelNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddToWishlistDto { HotelId = Guid.NewGuid() };
        _hotelRepoMock.Setup(x => x.GetByIdAsync(dto.HotelId)).ReturnsAsync((Hotel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddToWishlistAsync(userId, dto));
    }

    [Fact]
    public async Task AddToWishlistAsync_WhenAlreadyExists_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var dto = new AddToWishlistDto { HotelId = hotelId };
        
        _hotelRepoMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(new Hotel { Id = hotelId });
        _wishlistRepoMock.Setup(x => x.ExistsAsync(userId, hotelId)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddToWishlistAsync(userId, dto));
    }

    [Fact]
    public async Task IsInWishlistAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        _wishlistRepoMock.Setup(x => x.ExistsAsync(userId, hotelId)).ReturnsAsync(true);

        // Act
        var result = await _sut.IsInWishlistAsync(userId, hotelId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ToggleWishlistAsync_WhenExists_RemovesAndReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var wishlist = new Wishlist { Id = Guid.NewGuid(), UserId = userId, HotelId = hotelId };
        
        _wishlistRepoMock.Setup(x => x.ExistsAsync(userId, hotelId)).ReturnsAsync(true);
        _wishlistRepoMock.Setup(x => x.GetByUserAndHotelAsync(userId, hotelId)).ReturnsAsync(wishlist);
        _wishlistRepoMock.Setup(x => x.DeleteAsync(wishlist.Id)).ReturnsAsync(true);

        // Act
        var result = await _sut.ToggleWishlistAsync(userId, hotelId);

        // Assert
        Assert.False(result); // Removed
        _wishlistRepoMock.Verify(x => x.DeleteAsync(wishlist.Id), Times.Once);
    }

    [Fact]
    public async Task ToggleWishlistAsync_WhenNotExists_AddsAndReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test", Brand = new Brand { Name = "Brand" } };
        
        _wishlistRepoMock.Setup(x => x.ExistsAsync(userId, hotelId)).ReturnsAsync(false);
        _hotelRepoMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _wishlistRepoMock.Setup(x => x.CreateAsync(It.IsAny<Wishlist>())).ReturnsAsync(new Wishlist { HotelId = hotelId });
        _wishlistRepoMock.Setup(x => x.GetByUserAndHotelAsync(userId, hotelId))
            .ReturnsAsync(new Wishlist { HotelId = hotelId, Hotel = hotel });

        // Act
        var result = await _sut.ToggleWishlistAsync(userId, hotelId);

        // Assert
        Assert.True(result); // Added
    }
}
```

---

## ? Checklist

- [ ] Entity `Wishlist` ?ã t?o
- [ ] DbContext ?ã update v?i DbSet và configuration
- [ ] DTOs ?ã t?o (`WishlistDto.cs`)
- [ ] Mapper methods ?ã thêm
- [ ] `IWishlistRepository` ?ã t?o
- [ ] `WishlistRepository` ?ã implement
- [ ] `IWishlistService` ?ã t?o
- [ ] `WishlistService` ?ã implement
- [ ] `WishlistController` ?ã t?o
- [ ] Registered trong `Program.cs`
- [ ] Migration ?ã t?o
- [ ] Unit tests ?ã vi?t
- [ ] Build passes

---

## ?? API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/wishlist` | User | Get user's wishlist |
| GET | `/api/wishlist/check/{hotelId}` | User | Check if hotel is in wishlist |
| POST | `/api/wishlist` | User | Add hotel to wishlist |
| POST | `/api/wishlist/toggle/{hotelId}` | User | Toggle wishlist (add/remove) |
| PUT | `/api/wishlist/{hotelId}` | User | Update note |
| DELETE | `/api/wishlist/{hotelId}` | User | Remove from wishlist |
