# Phase 7: Dynamic Permission Module

## ?? Muc tieu

Xay dung he thong phan quyen dong (dynamic permission) voi scope (Brand, Hotel) dam bao:
- **SuperAdmin**: Chi quan ly he thong (Brands, Subscriptions, System config)
- **BrandAdmin**: Quan ly hotels trong brand cua minh
- **HotelManager**: Quan ly hotel duoc assign
- **Receptionist/Staff**: Quyen han theo permissions duoc gan

**Thay the `[Authorize(Roles = "...")]` bang `[Authorize(Policy = "Permission:xxx")]`

## ?? Correct Business Logic

### 7.1 Role Scope Definition

| Role | Scope | Quan han |
|------|-------|----------|
| **SuperAdmin** | System | Quan ly Brands, Subscriptions, System settings, Audit logs |
| **BrandAdmin** | Brand | CRUD Hotels trong brand, quan ly staff brand, xem brand reports |
| **HotelManager** | Hotel | CRUD Rooms, quan ly Bookings, Promotions cua hotel duoc assign |
| **Receptionist** | Hotel | Check-in/Check-out, quan ly Bookings (limited) |
| **Staff** | Hotel | Quyen han theo permissions duoc gan cu the |

### 7.2 Authorization Matrix (CORRECTED)

| Resource | SuperAdmin | BrandAdmin | HotelManager | Receptionist | Staff |
|----------|------------|------------|--------------|--------------|-------|
| Brands | CRUD (all) | Read (own) | - | - | - |
| Hotels | **Read (all)** | CRUD (own brand) | Read (own hotel) | Read (own hotel) | Read (own hotel) |
| Rooms | - | - | CRUD (own hotel) | Read | Read |
| Bookings | Read (all) | Read (own brand) | CRUD (own hotel) | Check-in/out | - |
| Users | CRUD (all) | CRUD (brand users) | CRUD (hotel users) | - | - |
| Subscriptions | CRUD (all) | Read | - | - | - |
| Dashboard | System stats | Brand stats | Hotel stats | Hotel stats | Hotel stats |

**QUAN TRONG**: SuperAdmin KHONG CRUD hotels - chi BrandAdmin moi co quyen nay!

---

## ?? Tasks

### Task 7.1: Permission Entities & Database
**Files can tao/sua:**

#### 1. `Models/Entities/Permission.cs` - Permission entity
```csharp
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;         // e.g., "hotels.create"
    public string Description { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;     // e.g., "hotels"
    public string Action { get; set; } = string.Empty;       // e.g., "create"
}
```

#### 2. `Models/Entities/RolePermission.cs` - Role-Permission mapping
```csharp
public class RolePermission : BaseEntity
{
    public UserRole Role { get; set; }
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
```

#### 3. `Models/Entities/UserHotelPermission.cs` - User override permission
```csharp
public class UserHotelPermission : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid HotelId { get; set; }
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
    public bool IsGranted { get; set; } = true;  // true = grant, false = deny
}
```

#### 4. `Data/ApplicationDbContext.cs` - Add DbSets
```csharp
// Add to ApplicationDbContext
public DbSet<Permission> Permissions { get; set; }
public DbSet<RolePermission> RolePermissions { get; set; }
public DbSet<UserHotelPermission> UserHotelPermissions { get; set; }
```

---

### Task 7.2: Permissions Static Class
**Files can tao:**

#### 1. `Models/Constants/Permissions.cs` - Permission constants
```csharp
public static class Permissions
{
    public static class Hotels
    {
        public const string Create = "hotels.create";
        public const string Read = "hotels.read";
        public const string Update = "hotels.update";
        public const string Delete = "hotels.delete";
    }

    public static class Rooms
    {
        public const string Create = "rooms.create";
        public const string Read = "rooms.read";
        public const string Update = "rooms.update";
        public const string Delete = "rooms.delete";
    }

    public static class Bookings
    {
        public const string Create = "bookings.create";
        public const string Read = "bookings.read";
        public const string Update = "bookings.update";
        public const string Delete = "bookings.delete";
        public const string CheckIn = "bookings.checkin";
        public const string CheckOut = "bookings.checkout";
    }

    public static class Users
    {
        public const string Create = "users.create";
        public const string Read = "users.read";
        public const string Update = "users.update";
        public const string Delete = "users.delete";
    }

    public static class Brands
    {
        public const string Create = "brands.create";
        public const string Read = "brands.read";
        public const string Update = "brands.update";
        public const string Delete = "brands.delete";
    }

    public static class Subscriptions
    {
        public const string Read = "subscriptions.read";
        public const string Update = "subscriptions.update";
    }

    public static class Dashboard
    {
        public const string View = "dashboard.view";
        public const string ViewAll = "dashboard.viewall";
    }
}
```

---

### Task 7.3: Permission Service
**Files can tao:**

#### 1. `Interfaces/Services/IPermissionService.cs`
```csharp
public interface IPermissionService
{
    Task<List<Permission>> GetAllPermissionsAsync();
    Task<List<string>> GetUserPermissionsAsync(Guid userId);
    Task<List<string>> GetUserPermissionsForHotelAsync(Guid userId, Guid hotelId);
    Task<bool> HasPermissionAsync(Guid userId, string permission);
    Task<bool> HasPermissionForHotelAsync(Guid userId, Guid hotelId, string permission);
    Task GrantHotelPermissionAsync(Guid userId, Guid hotelId, string permission);
    Task RevokeHotelPermissionAsync(Guid userId, Guid hotelId, string permission);
}
```

#### 2. `Services/PermissionService.cs`
```csharp
public class PermissionService(
    ApplicationDbContext context,
    IUserRepository userRepository) : IPermissionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _context.Permissions.AsNoTracking().ToListAsync();
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new List<string>();

        // SuperAdmin has all system permissions
        if (user.Role == UserRole.SuperAdmin)
        {
            return await _context.Permissions
                .Where(p => p.Resource != "hotel")  // Hotel-specific permissions excluded
                .Select(p => p.Name)
                .ToListAsync();
        }

        // Get role permissions
        var rolePermissions = await _context.RolePermissions
            .Where(rp => rp.Role == user.Role)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return rolePermissions.Distinct().ToList();
    }

    public async Task<List<string>> GetUserPermissionsForHotelAsync(Guid userId, Guid hotelId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new List<string>();

        // SuperAdmin has all permissions for all hotels
        if (user.Role == UserRole.SuperAdmin)
        {
            return await _context.Permissions.Select(p => p.Name).ToListAsync();
        }

        var basePermissions = await GetUserPermissionsAsync(userId);

        // Get user hotel-specific permissions
        var hotelPermissions = await _context.UserHotelPermissions
            .Where(uhp => uhp.UserId == userId && uhp.HotelId == hotelId)
            .Select(uhp => uhp.Permission.Name)
            .ToListAsync();

        // Merge: base + hotel-specific (hotel-specific overrides)
        return basePermissions.Union(hotelPermissions).Distinct().ToList();
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permission);
    }

    public async Task<bool> HasPermissionForHotelAsync(Guid userId, Guid hotelId, string permission)
    {
        var permissions = await GetUserPermissionsForHotelAsync(userId, hotelId);
        return permissions.Contains(permission);
    }

    public async Task GrantHotelPermissionAsync(Guid userId, Guid hotelId, string permission)
    {
        var permissionEntity = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permission);

        if (permissionEntity == null) return;

        var existing = await _context.UserHotelPermissions
            .FirstOrDefaultAsync(uhp =>
                uhp.UserId == userId &&
                uhp.HotelId == hotelId &&
                uhp.PermissionId == permissionEntity.Id);

        if (existing != null)
        {
            existing.IsGranted = true;
        }
        else
        {
            _context.UserHotelPermissions.Add(new UserHotelPermission
            {
                UserId = userId,
                HotelId = hotelId,
                PermissionId = permissionEntity.Id,
                IsGranted = true
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokeHotelPermissionAsync(Guid userId, Guid hotelId, string permission)
    {
        var permissionEntity = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permission);

        if (permissionEntity == null) return;

        var existing = await _context.UserHotelPermissions
            .FirstOrDefaultAsync(uhp =>
                uhp.UserId == userId &&
                uhp.HotelId == hotelId &&
                uhp.PermissionId == permissionEntity.Id);

        if (existing != null)
        {
            existing.IsGranted = false;
            await _context.SaveChangesAsync();
        }
    }
}
```

---

### Task 7.4: Permission Context (Scoped Service)
**Files can tao:**

#### 1. `Services/PermissionContext.cs`
```csharp
public class PermissionContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionService _permissionService;
    private readonly IUserRepository _userRepository;

    private User? _cachedUser;

    public PermissionContext(
        IHttpContextAccessor httpContextAccessor,
        IPermissionService permissionService,
        IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _permissionService = permissionService;
        _userRepository = userRepository;
    }

    private Guid UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("userId");
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    public UserRole Role
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            return claim != null ? Enum.Parse<UserRole>(claim.Value) : UserRole.Guest;
        }
    }

    public Guid? BrandId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("brandId");
            return claim != null && !string.IsNullOrEmpty(claim.Value)
                ? Guid.Parse(claim.Value) : null;
        }
    }

    public Guid? HotelId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("hotelId");
            return claim != null && !string.IsNullOrEmpty(claim.Value)
                ? Guid.Parse(claim.Value) : null;
        }
    }

    public bool IsSuperAdmin => Role == UserRole.SuperAdmin;
    public bool IsBrandAdmin => Role == UserRole.BrandAdmin;
    public bool IsHotelManager => Role == UserRole.HotelManager;
    public bool IsReceptionist => Role == UserRole.Receptionist;

    public async Task<bool> CanAccessAsync(string permission)
    {
        // SuperAdmin co quyen he thong (ngoai tru hotel-specific)
        if (IsSuperAdmin && !permission.StartsWith("hotel"))
            return true;

        return await _permissionService.HasPermissionAsync(UserId, permission);
    }

    public async Task<bool> CanAccessHotelAsync(Guid hotelId, string permission)
    {
        // SuperAdmin chi duoc xem, khong duoc quan ly
        if (IsSuperAdmin)
        {
            // SuperAdmin co the xem tat ca hotels nhung khong modify
            return permission.EndsWith(".read");
        }

        // BrandAdmin: chi truy cap hotels trong brand cua minh
        if (IsBrandAdmin)
        {
            if (BrandId.HasValue)
            {
                // Check hotel thuoc brand
                var hotel = await _context.Hotels.FindAsync(hotelId);
                if (hotel?.BrandId == BrandId.Value)
                    return true;
            }
        }

        // HotelManager: chi truy cap hotel duoc assign
        if (IsHotelManager && HotelId == hotelId)
            return true;

        // Receptionist/Staff: check permission for specific hotel
        return await _permissionService.HasPermissionForHotelAsync(UserId, hotelId, permission);
    }

    public async Task<bool> CanManageHotelsInBrandAsync(Guid brandId)
    {
        if (IsSuperAdmin) return true;
        if (IsBrandAdmin && BrandId == brandId) return true;
        return false;
    }

    public User GetCurrentUser()
    {
        if (_cachedUser == null)
            _cachedUser = _userRepository.GetByIdAsync(UserId).Result;
        return _cachedUser!;
    }

    private ApplicationDbContext _context;
}
```

---

### Task 7.5: Custom Authorization Handler
**Files can tao:**

#### 1. `Authorization/PermissionRequirement.cs`
```csharp
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
```

#### 2. `Authorization/PermissionHandler.cs`
```csharp
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        if (httpContext == null) return;

        using var scope = _serviceProvider.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        var userIdClaim = httpContext.User.FindFirst("userId");
        if (userIdClaim == null) return;

        var userId = Guid.Parse(userIdClaim.Value);
        var permission = requirement.Permission;

        // Kiem tra permission voi hotel scope
        var hotelIdParam = httpContext.Request.RouteValues["hotelId"]?.ToString();
        if (hotelIdParam != null && Guid.TryParse(hotelIdParam, out var hotelId))
        {
            if (await permissionService.HasPermissionForHotelAsync(userId, hotelId, permission))
                context.Succeed(requirement);
        }
        else
        {
            if (await permissionService.HasPermissionAsync(userId, permission))
                context.Succeed(requirement);
        }
    }
}
```

#### 3. `Program.cs` - Register authorization services
```csharp
// Register Permission Policy Provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

// Register Permission Handler
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
```

---

### Task 7.6: Update JWT Service - Add Permissions to Token
**Files can sua:**

#### `Services/JwtService.cs`
```csharp
public string GenerateAccessToken(User user)
{
    var permissions = GetUserPermissions(user);

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.GivenName, user.FirstName),
        new(ClaimTypes.Surname, user.LastName),
        new(ClaimTypes.Role, user.Role.ToString()),
        new("userId", user.Id.ToString()),
        new("brandId", user.BrandId?.ToString() ?? ""),
        new("hotelId", user.HotelId?.ToString() ?? ""),
        new("permissions", string.Join(",", permissions)),
        new("scope", GetScope(user.Role))
    };

    // ... rest of token generation
}

private List<string> GetUserPermissions(User user)
{
    // Return pre-computed permissions based on role
    return user.Role switch
    {
        UserRole.SuperAdmin => new List<string>
        {
            Permissions.Brands.Create, Permissions.Brands.Read, Permissions.Brands.Update, Permissions.Brands.Delete,
            Permissions.Subscriptions.Read, Permissions.Subscriptions.Update,
            Permissions.Users.Create, Permissions.Users.Read, Permissions.Users.Update, Permissions.Users.Delete,
            Permissions.Dashboard.ViewAll
        },
        UserRole.BrandAdmin => new List<string>
        {
            Permissions.Brands.Read,
            Permissions.Hotels.Create, Permissions.Hotels.Read, Permissions.Hotels.Update, Permissions.Hotels.Delete,
            Permissions.Users.Create, Permissions.Users.Read, Permissions.Users.Update, Permissions.Users.Delete,
            Permissions.Dashboard.View
        },
        UserRole.HotelManager => new List<string>
        {
            Permissions.Rooms.Create, Permissions.Rooms.Read, Permissions.Rooms.Update, Permissions.Rooms.Delete,
            Permissions.Bookings.Create, Permissions.Bookings.Read, Permissions.Bookings.Update, Permissions.Bookings.Delete,
            Permissions.Bookings.CheckIn, Permissions.Bookings.CheckOut,
            Permissions.Dashboard.View
        },
        UserRole.Receptionist => new List<string>
        {
            Permissions.Rooms.Read,
            Permissions.Bookings.Read, Permissions.Bookings.CheckIn, Permissions.Bookings.CheckOut
        },
        _ => new List<string> { Permissions.Hotels.Read }
    };
}

private string GetScope(UserRole role)
{
    return role switch
    {
        UserRole.SuperAdmin => "system",
        UserRole.BrandAdmin => "brand",
        _ => "hotel"
    };
}
```

---

### Task 7.7: Update Controllers - Permission-based Authorization
**Files can sua:**

#### 1. `Controllers/UsersController.cs`
```csharp
[Authorize(Policy = "Permission:users.read")]
public async Task<ActionResult<ApiResponseDto<PagedResultDto<UserDto>>>> GetUsers([FromQuery] UserQueryDto query)
{
    var currentUser = _permissionContext.GetCurrentUser();

    // SuperAdmin: xem tat ca users
    if (currentUser.IsSuperAdmin)
        return Ok(await _userService.GetAllAsync(query));

    // BrandAdmin: xem users trong brand cua minh
    if (currentUser.IsBrandAdmin && currentUser.BrandId.HasValue)
    {
        query.BrandId = currentUser.BrandId.Value;
        return Ok(await _userService.GetByBrandAsync(query));
    }

    // HotelManager: xem users trong hotel cua minh
    if (currentUser.IsHotelManager && currentUser.HotelId.HasValue)
    {
        query.HotelId = currentUser.HotelId.Value;
        return Ok(await _userService.GetByHotelAsync(query));
    }

    return Forbid();
}
```

#### 2. `Controllers/HotelsController.cs`
```csharp
// SuperAdmin KHONG CRUD hotels - chi BrandAdmin moi co quyen nay!

[HttpGet]
[Authorize(Policy = "Permission:hotels.read")]
public async Task<ActionResult<ApiResponseDto<PagedResultDto<HotelDto>>>> GetHotels([FromQuery] HotelQueryDto query)
{
    // Tat ca roles deu co the search hotels
    var result = await _hotelService.GetAllAsync(query);
    return Ok(new ApiResponseDto<PagedResultDto<HotelDto>>
    {
        Success = true,
        Data = result
    });
}

[HttpPost]
[Authorize(Policy = "Permission:hotels.create")]
public async Task<ActionResult<ApiResponseDto<HotelDto>>> CreateHotel([FromBody] CreateHotelDto dto)
{
    var currentUser = _permissionContext.GetCurrentUser();

    // Chi BrandAdmin moi tao duoc hotel
    if (!currentUser.IsBrandAdmin)
        return Forbid(new ApiResponseDto<HotelDto>
        {
            Success = false,
            Message = "Only BrandAdmin can create hotels"
        });

    dto.BrandId = currentUser.BrandId!.Value;
    var result = await _hotelService.CreateAsync(dto);

    return Ok(new ApiResponseDto<HotelDto>
    {
        Success = true,
        Data = result
    });
}

[HttpPut("{id}")]
[Authorize(Policy = "Permission:hotels.update")]
public async Task<ActionResult<ApiResponseDto<HotelDto>>> UpdateHotel(Guid id, [FromBody] UpdateHotelDto dto)
{
    var currentUser = _permissionContext.GetCurrentUser();

    // Kiem tra quyen
    if (!await _permissionContext.CanAccessHotelAsync(id, Permissions.Hotels.Update))
        return Forbid();

    var result = await _hotelService.UpdateAsync(id, dto);
    if (result == null)
        return NotFound();

    return Ok(new ApiResponseDto<HotelDto> { Success = true, Data = result });
}

[HttpDelete("{id}")]
[Authorize(Policy = "Permission:hotels.delete")]
public async Task<ActionResult<ApiResponseDto>> DeleteHotel(Guid id)
{
    var currentUser = _permissionContext.GetCurrentUser();

    // Chi BrandAdmin moi xoa duoc hotel
    if (!currentUser.IsBrandAdmin)
        return Forbid();

    await _hotelService.DeleteAsync(id);
    return Ok(new ApiResponseDto { Success = true });
}
```

#### 3. `Controllers/DashboardController.cs`
```csharp
[Authorize(Policy = "Permission:dashboard.view")]
public async Task<ActionResult<ApiResponseDto<DashboardStatsDto>>> GetDashboardStats()
{
    var currentUser = _permissionContext.GetCurrentUser();

    if (currentUser.IsSuperAdmin)
    {
        // System-wide stats
        var stats = await _dashboardService.GetSystemStatsAsync();
        return Ok(new ApiResponseDto<DashboardStatsDto> { Success = true, Data = stats });
    }

    if (currentUser.IsBrandAdmin && currentUser.BrandId.HasValue)
    {
        // Brand stats
        var stats = await _dashboardService.GetBrandStatsAsync(currentUser.BrandId.Value);
        return Ok(new ApiResponseDto<DashboardStatsDto> { Success = true, Data = stats });
    }

    if (currentUser.HotelId.HasValue)
    {
        // Hotel stats
        var stats = await _dashboardService.GetHotelStatsAsync(currentUser.HotelId.Value);
        return Ok(new ApiResponseDto<DashboardStatsDto> { Success = true, Data = stats });
    }

    return Forbid();
}
```

---

### Task 7.8: Seed Data - Default Permissions
**Files can sua:**

#### `Data/SeedData.cs` - Add Permission seeding
```csharp
public static async Task SeedPermissionsAsync(ApplicationDbContext context)
{
    if (await context.Permissions.AnyAsync()) return;

    var permissions = new List<Permission>
    {
        // Hotels
        new() { Name = Permissions.Hotels.Create, Description = "Create hotels", Resource = "hotels", Action = "create" },
        new() { Name = Permissions.Hotels.Read, Description = "View hotels", Resource = "hotels", Action = "read" },
        new() { Name = Permissions.Hotels.Update, Description = "Update hotels", Resource = "hotels", Action = "update" },
        new() { Name = Permissions.Hotels.Delete, Description = "Delete hotels", Resource = "hotels", Action = "delete" },

        // Rooms
        new() { Name = Permissions.Rooms.Create, Description = "Create rooms", Resource = "rooms", Action = "create" },
        new() { Name = Permissions.Rooms.Read, Description = "View rooms", Resource = "rooms", Action = "read" },
        new() { Name = Permissions.Rooms.Update, Description = "Update rooms", Resource = "rooms", Action = "update" },
        new() { Name = Permissions.Rooms.Delete, Description = "Delete rooms", Resource = "rooms", Action = "delete" },

        // Bookings
        new() { Name = Permissions.Bookings.Create, Description = "Create bookings", Resource = "bookings", Action = "create" },
        new() { Name = Permissions.Bookings.Read, Description = "View bookings", Resource = "bookings", Action = "read" },
        new() { Name = Permissions.Bookings.Update, Description = "Update bookings", Resource = "bookings", Action = "update" },
        new() { Name = Permissions.Bookings.Delete, Description = "Delete bookings", Resource = "bookings", Action = "delete" },
        new() { Name = Permissions.Bookings.CheckIn, Description = "Check-in guests", Resource = "bookings", Action = "checkin" },
        new() { Name = Permissions.Bookings.CheckOut, Description = "Check-out guests", Resource = "bookings", Action = "checkout" },

        // Users
        new() { Name = Permissions.Users.Create, Description = "Create users", Resource = "users", Action = "create" },
        new() { Name = Permissions.Users.Read, Description = "View users", Resource = "users", Action = "read" },
        new() { Name = Permissions.Users.Update, Description = "Update users", Resource = "users", Action = "update" },
        new() { Name = Permissions.Users.Delete, Description = "Delete users", Resource = "users", Action = "delete" },

        // Brands
        new() { Name = Permissions.Brands.Create, Description = "Create brands", Resource = "brands", Action = "create" },
        new() { Name = Permissions.Brands.Read, Description = "View brands", Resource = "brands", Action = "read" },
        new() { Name = Permissions.Brands.Update, Description = "Update brands", Resource = "brands", Action = "update" },
        new() { Name = Permissions.Brands.Delete, Description = "Delete brands", Resource = "brands", Action = "delete" },

        // Subscriptions
        new() { Name = Permissions.Subscriptions.Read, Description = "View subscriptions", Resource = "subscriptions", Action = "read" },
        new() { Name = Permissions.Subscriptions.Update, Description = "Update subscriptions", Resource = "subscriptions", Action = "update" },

        // Dashboard
        new() { Name = Permissions.Dashboard.View, Description = "View dashboard", Resource = "dashboard", Action = "view" },
        new() { Name = Permissions.Dashboard.ViewAll, Description = "View all dashboards", Resource = "dashboard", Action = "viewall" }
    };

    context.Permissions.AddRange(permissions);
    await context.SaveChangesAsync();
}
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/PermissionServiceTests.cs`

```csharp
using Xunit;
using Moq;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class PermissionServiceTests
{
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly PermissionService _sut;

    public PermissionServiceTests()
    {
        _contextMock = new Mock<ApplicationDbContext>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _sut = new PermissionService(_contextMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task HasPermissionAsync_SuperAdmin_ReturnsTrue_ForSystemPermissions()
    {
        // Arrange
        var superAdmin = new User
        {
            Id = Guid.NewGuid(),
            Role = UserRole.SuperAdmin,
            Email = "admin@hotelsaas.com"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(superAdmin.Id)).ReturnsAsync(superAdmin);

        // Act
        var result = await _sut.HasPermissionAsync(superAdmin.Id, "brands.read");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_HotelManager_ReturnsFalse_ForHotelCreate()
    {
        // Arrange
        var manager = new User
        {
            Id = Guid.NewGuid(),
            Role = UserRole.HotelManager,
            HotelId = Guid.NewGuid()
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(manager.Id)).ReturnsAsync(manager);

        // Act
        var result = await _sut.HasPermissionAsync(manager.Id, "hotels.create");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionForHotelAsync_BrandAdmin_CanAccessOwnBrandHotel()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var brandAdmin = new User
        {
            Id = Guid.NewGuid(),
            Role = UserRole.BrandAdmin,
            BrandId = brandId
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(brandAdmin.Id)).ReturnsAsync(brandAdmin);

        // Mock hotels set
        var hotelMock = new Mock<DbSet<Hotel>>();
        var hotel = new Hotel { Id = hotelId, BrandId = brandId };
        hotelMock.Setup(m => m.FindAsync(hotelId)).ReturnsAsync(hotel);
        _contextMock.Setup(c => c.Hotels).Returns(hotelMock.Object);

        // Act
        var result = await _sut.HasPermissionForHotelAsync(brandAdmin.Id, hotelId, "hotels.read");

        // Assert
        Assert.True(result);
    }
}
```

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/PermissionContextTests.cs`

```csharp
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class PermissionContextTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly PermissionContext _sut;

    public PermissionContextTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _permissionServiceMock = new Mock<IPermissionService>();
        _userRepositoryMock = new Mock<IUserRepository>();

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("userId", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, UserRole.BrandAdmin.ToString()),
                new Claim("brandId", Guid.NewGuid().ToString())
            })
        });
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _sut = new PermissionContext(
            _httpContextAccessorMock.Object,
            _permissionServiceMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public void IsSuperAdmin_WhenRoleIsSuperAdmin_ReturnsTrue()
    {
        // Arrange
        var superAdminContext = new DefaultHttpContext();
        superAdminContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, UserRole.SuperAdmin.ToString())
            })
        });
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(superAdminContext);

        var superAdminContextService = new PermissionContext(
            _httpContextAccessorMock.Object,
            _permissionServiceMock.Object,
            _userRepositoryMock.Object);

        // Act & Assert
        Assert.True(superAdminContextService.IsSuperAdmin);
    }

    [Fact]
    public void IsBrandAdmin_WhenRoleIsBrandAdmin_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(_sut.IsBrandAdmin);
    }

    [Fact]
    public async Task CanAccessAsync_WhenSuperAdmin_ReturnsTrue_ForSystemPermission()
    {
        // Act
        var result = await _sut.CanAccessAsync("brands.read");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanManageHotelsInBrandAsync_WhenBrandAdmin_ReturnsTrue_ForOwnBrand()
    {
        // Arrange
        var brandId = Guid.Parse(_httpContextAccessorMock.Object.HttpContext!.User.FindFirst("brandId")!.Value);

        // Act
        var result = await _sut.CanManageHotelsInBrandAsync(brandId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanManageHotelsInBrandAsync_WhenBrandAdmin_ReturnsFalse_ForOtherBrand()
    {
        // Arrange
        var otherBrandId = Guid.NewGuid();

        // Act
        var result = await _sut.CanManageHotelsInBrandAsync(otherBrandId);

        // Assert
        Assert.False(result);
    }
}
```

---

## ?? Checklist

- [ ] `Permission` entity da tao
- [ ] `RolePermission` entity da tao
- [ ] `UserHotelPermission` entity da tao
- [ ] `Permissions` constants class da tao
- [ ] `IPermissionService` interface da tao
- [ ] `PermissionService` da implement
- [ ] `PermissionContext` scoped service da tao
- [ ] `PermissionRequirement` da tao
- [ ] `PermissionHandler` da tao
- [ ] `Program.cs` updated with authorization services
- [ ] `JwtService` updated to include permissions in token
- [ ] `UsersController` updated with permission-based auth
- [ ] `HotelsController` updated - SuperAdmin cannot CRUD hotels
- [ ] `DashboardController` updated with scope-based stats
- [ ] `SeedData` - Permission seeding da them
- [ ] Migration da tao
- [ ] Unit tests da viet
- [ ] Build passes
- [ ] Manual test via Swagger

---

## ?? API Endpoints sau khi hoan thanh

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/hotels` | Permission:hotels.read | Search hotels (all roles) |
| POST | `/api/hotels` | Permission:hotels.create + BrandAdmin | Create hotel (BrandAdmin only) |
| PUT | `/api/hotels/{id}` | Permission:hotels.update + scope check | Update hotel |
| DELETE | `/api/hotels/{id}` | Permission:hotels.delete + BrandAdmin | Delete hotel (BrandAdmin only) |
| GET | `/api/users` | Permission:users.read + scope check | Get users (role-based scope) |
| GET | `/api/dashboard/stats` | Permission:dashboard.view + scope check | Get stats (system/brand/hotel) |

---

## ?? Business Logic Summary

| Role | Hotels | Brand | Subscriptions | Users | Dashboard |
|------|--------|-------|---------------|-------|-----------|
| SuperAdmin | Read only | CRUD all | CRUD all | CRUD all | System |
| BrandAdmin | CRUD own brand | Read own | Read | CRUD brand users | Brand |
| HotelManager | Read own hotel | - | - | CRUD hotel users | Hotel |
| Receptionist | Read own hotel | - | - | - | Hotel (view only) |

**Key Rule**: SuperAdmin KHONG duoc phep CRUD hotels truc tiep!
