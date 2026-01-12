# Hotel SAAS Backend - Implementation Plan

## ?? T?ng quan

?�y l� Implementation Plan ?? n�ng c?p Hotel SAAS Backend th�nh m?t MVP ho�n ch?nh gi?ng Traveloka.

**Nguy�n t?c ch�nh:**
- MVP mindset: L�m ?? d�ng, kh�ng over-engineer
- Code ??n gi?n, d? ??c, d? maintain
- Tu�n th? convention hi?n t?i c?a project
- M?i feature ph?i c� test c? b?n

---

## ??? Code Convention (B?t bu?c tu�n th?)

### C?u tr�c th? m?c
```
Hotel-SAAS-Backend.API/
??? Controllers/           # API endpoints - ch? handle HTTP, g?i Service
??? Services/              # Business logic - x? l� nghi?p v?
??? Repositories/          # Data access - LINQ queries
??? Interfaces/
?   ??? Services/          # IXxxService
?   ??? Repositories/      # IXxxRepository
??? Models/
?   ??? Entities/          # Database models
?   ??? DTOs/              # Request/Response objects
?   ??? Enums/             # Enumerations
?   ??? Options/           # Configuration classes
??? Mapping/               # Mapper.cs - static mapping methods
??? Data/                  # DbContext, SeedData
??? Migrations/            # EF Core migrations
```

### Naming Convention
| Lo?i | Convention | V� d? |
|------|------------|-------|
| Interface | `I` + PascalCase | `IHotelService`, `IBookingRepository` |
| Service | PascalCase + `Service` | `BookingService`, `PromotionService` |
| Repository | PascalCase + `Repository` | `HotelRepository` |
| Controller | PascalCase + `Controller` | `HotelsController` (s? nhi?u) |
| DTO Request | `Create/Update` + Entity + `Dto` | `CreatePromotionDto` |
| DTO Response | Entity + `Dto` / Entity + `DetailDto` | `PromotionDto`, `BookingDetailDto` |
| Entity | PascalCase (s? �t) | `Promotion`, `Wishlist` |
| Enum | PascalCase | `PromotionType`, `DiscountType` |

### Code Style
```csharp
// ? Primary constructor (C# 12)
public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    // Direct field usage, no private field declaration needed
}

// ? Async method naming
public async Task<BookingDto?> GetBookingByIdAsync(Guid id)

// ? Nullable v?i ?
public string? Description { get; set; }

// ? Response wrapper
return Ok(new ApiResponseDto<T> { Success = true, Data = result });

// ? Simple null check
if (entity == null) return null;

// ? LINQ Select v?i Mapper
return bookings.Select(Mapper.ToDto);
```

### Mapping Pattern
```csharp
// Trong Mapper.cs - static methods
public static PromotionDto ToDto(Promotion entity) { ... }
public static Promotion ToEntity(CreatePromotionDto dto) { ... }
public static void UpdateEntity(UpdatePromotionDto dto, Promotion entity) { ... }
```

### Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<PromotionDto>>> GetById(Guid id)
    {
        var result = await _promotionService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new ApiResponseDto<PromotionDto> { Success = false, Message = "Not found" });
        
        return Ok(new ApiResponseDto<PromotionDto> { Success = true, Data = result });
    }
}
```

### Comment Rules
- Code ng?n, t? gi?i th�ch ? KH�NG comment
- Code ph?c t?p, logic nghi?p v? ? Comment gi?i th�ch WHY, kh�ng ph?i WHAT
- Region comments cho Mapper.cs: `#region Promotion Mappings`

---

## ?? Implementation Phases

### Phase 1: Search & Availability (P0 - Critical)
**M?c ti�u:** User c� th? search hotels v� check room availability

| Task | File | Priority |
|------|------|----------|
| Advanced Search v?i filters | `HotelsController`, `HotelService` | P0 |
| Pagination support | `CommonDto.cs` | P0 |
| Room Availability checking | `RoomService`, `BookingRepository` | P0 |
| Search by date range | `HotelRepository` | P0 |

**Deliverables:**
- `GET /api/hotels/search` v?i pagination, filters
- `GET /api/hotels/{id}/availability?checkIn=&checkOut=`
- `PagedResultDto<T>` generic wrapper

---

### Phase 2: Promotions & Coupons (P1 - Important)
**M?c ti�u:** H? th?ng khuy?n m�i v� m� gi?m gi�

| Task | File | Priority |
|------|------|----------|
| Promotion entity & enum | `Models/Entities/Promotion.cs` | P1 |
| Coupon entity | `Models/Entities/Coupon.cs` | P1 |
| UserCoupon (tracking usage) | `Models/Entities/UserCoupon.cs` | P1 |
| PromotionService | `Services/PromotionService.cs` | P1 |
| Apply coupon to booking | `BookingService.cs` | P1 |

**Deliverables:**
- CRUD promotions (admin)
- Validate & apply coupon code
- Auto-calculate discount in booking

---

### Phase 3: Wishlist & Favorites (P1 - Important)
**M?c ti�u:** User c� th? save hotels y�u th�ch

| Task | File | Priority |
|------|------|----------|
| Wishlist entity | `Models/Entities/Wishlist.cs` | P1 |
| WishlistService | `Services/WishlistService.cs` | P1 |
| WishlistController | `Controllers/WishlistController.cs` | P1 |

**Deliverables:**
- Add/Remove hotel from wishlist
- Get user's wishlist
- Check if hotel is in wishlist

---

### Phase 4: Notifications (P1 - Important)
**M?c ti�u:** G?i email notifications cho booking events

| Task | File | Priority |
|------|------|----------|
| Notification entity | `Models/Entities/Notification.cs` | P1 |
| Email templates | `Services/Email/` | P1 |
| NotificationService | `Services/NotificationService.cs` | P1 |
| Integration v?i Booking | `BookingService.cs` | P1 |

**Deliverables:**
- Email: Booking confirmation, Reminder, Cancellation
- In-app notifications list
- Mark as read

---

### Phase 5: Dashboard & Analytics (P2 - Nice to have)
**M?c ti�u:** Basic dashboard cho hotel managers

| Task | File | Priority |
|------|------|----------|
| DashboardService | `Services/DashboardService.cs` | P2 |
| Statistics DTOs | `Models/DTOs/DashboardDto.cs` | P2 |
| DashboardController | `Controllers/DashboardController.cs` | P2 |

**Deliverables:**
- Revenue summary (today, week, month)
- Booking statistics
- Occupancy rate
- Top performing rooms

---

### Phase 6: Guest Profile Enhancement (P2 - Nice to have)
**M?c ti�u:** N�ng cao guest profile

| Task | File | Priority |
|------|------|----------|
| Booking history v?i filters | `BookingsController.cs` | P2 |
| Recently viewed hotels | `RecentlyViewedService.cs` | P2 |
| Guest preferences | `User.cs` enhancement | P2 |

---

### Phase 7: Dynamic Permission Module (P0 - Critical)
**Muc tieu:** He thong phan quyen dong voi scope (Brand, Hotel) - SuperAdmin KHONG CRUD hotels!

| Task | File | Priority |
|------|------|----------|
| Permission entities | `Models/Entities/Permission.cs` | P0 |
| RolePermission mapping | `Models/Entities/RolePermission.cs` | P0 |
| UserHotelPermission override | `Models/Entities/UserHotelPermission.cs` | P0 |
| Permissions constants | `Models/Constants/Permissions.cs` | P0 |
| PermissionService | `Services/PermissionService.cs` | P0 |
| PermissionContext | `Services/PermissionContext.cs` | P0 |
| Custom authorization handler | `Authorization/PermissionHandler.cs` | P0 |
| Update JWT Service | `Services/JwtService.cs` | P0 |
| Update Controllers | `HotelsController`, `UsersController` | P0 |
| Seed permissions | `Data/SeedData.cs` | P0 |

**Deliverables:**
- Dynamic permission system thay the role-based cung
- SuperAdmin chi quan ly he thong, khong CRUD hotels
- BrandAdmin chi quan ly hotels trong brand cua minh
- HotelManager chi quan ly hotel duoc assign
- Scope-based data access

---

## ?? Testing Strategy

### Test Project Structure
```
Hotel-SAAS-Backend.Tests/
??? Unit/
?   ??? Services/
?   ?   ??? BookingServiceTests.cs
?   ?   ??? PromotionServiceTests.cs
?   ?   ??? ...
?   ??? Mapping/
?       ??? MapperTests.cs
??? Integration/
?   ??? Controllers/
?   ?   ??? HotelsControllerTests.cs
?   ?   ??? ...
?   ??? Repositories/
?       ??? ...
??? Fixtures/
    ??? TestDbContextFixture.cs
    ??? TestDataBuilder.cs
```

### Test Convention
```csharp
// Naming: MethodName_Scenario_ExpectedResult
[Fact]
public async Task GetByIdAsync_WhenExists_ReturnsDto()

[Fact]
public async Task ApplyCoupon_WhenExpired_ThrowsException()

// Arrange - Act - Assert pattern
[Fact]
public async Task CreateBooking_WithValidData_ReturnsBookingDto()
{
    // Arrange
    var dto = new CreateBookingDto { ... };
    
    // Act
    var result = await _service.CreateBookingAsync(dto);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(BookingStatus.Pending, result.Status);
}
```

### Minimum Test Coverage per Feature
- Service methods: Happy path + 1 error case
- Mapper methods: 1 test per mapping
- Controller: 1 integration test per endpoint (optional for MVP)

---

## ?? Detailed Plans

Xem c�c file plan chi ti?t:
1. [Phase 1: Search & Availability](./plans/PHASE1_SEARCH_AVAILABILITY.md)
2. [Phase 2: Promotions & Coupons](./plans/PHASE2_PROMOTIONS.md)
3. [Phase 3: Wishlist & Favorites](./plans/PHASE3_WISHLIST.md)
4. [Phase 4: Notifications](./plans/PHASE4_NOTIFICATIONS.md)
5. [Phase 5: Dashboard & Analytics](./plans/PHASE5_DASHBOARD.md)
6. [Phase 6: Guest Profile](./plans/PHASE6_GUEST_PROFILE.md)
7. [Phase 7: Dynamic Permission](./plans/PHASE7_DYNAMIC_PERMISSION.md)

---

## ? Definition of Done (per feature)

- [ ] Entity created v?i proper relationships
- [ ] DTOs created (Request + Response)
- [ ] Mapper methods added
- [ ] Repository interface + implementation
- [ ] Service interface + implementation
- [ ] Controller v?i proper authorization
- [ ] Registered in Program.cs (DI)
- [ ] Migration created (if DB changes)
- [ ] Basic unit tests written
- [ ] Build passes without errors
- [ ] Manual API test via Swagger

---

## ?? Quick Start

```bash
# 1. Checkout feature branch
git checkout -b feature/phase1-search

# 2. Implement theo plan

# 3. Run tests
dotnet test

# 4. Build
dotnet build

# 5. Commit & PR
git add .
git commit -m "feat: implement advanced hotel search with availability"
```

---

## ?? Progress Tracking

| Phase | Status | Started | Completed |
|-------|--------|---------|-----------|
| Phase 1: Search & Availability | ? Pending | - | - |
| Phase 2: Promotions | ? Pending | - | - |
| Phase 3: Wishlist | ? Pending | - | - |
| Phase 4: Notifications | ? Pending | - | - |
| Phase 5: Dashboard | ? Pending | - | - |
| Phase 6: Guest Profile | ? Pending | - | - |
| Phase 7: Dynamic Permission | ? Pending | - | - |
