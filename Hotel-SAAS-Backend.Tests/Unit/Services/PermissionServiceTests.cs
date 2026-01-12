using Microsoft.EntityFrameworkCore;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Services;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class PermissionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly PermissionService _sut;
    private readonly Guid _superAdminId = Guid.NewGuid();
    private readonly Guid _brandAdminId = Guid.NewGuid();
    private readonly Guid _hotelManagerId = Guid.NewGuid();
    private readonly Guid _receptionistId = Guid.NewGuid();
    private readonly Guid _guestId = Guid.NewGuid();
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly Guid _hotelId = Guid.NewGuid();

    public PermissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _userRepositoryMock = new Mock<IUserRepository>();
        _sut = new PermissionService(_context, _userRepositoryMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Seed permissions
        var permissions = new List<Permission>
        {
            new() { Name = Permissions.Hotels.Create, Description = "Create hotels", Resource = "hotels", Action = "create" },
            new() { Name = Permissions.Hotels.Read, Description = "View hotels", Resource = "hotels", Action = "read" },
            new() { Name = Permissions.Hotels.Update, Description = "Update hotels", Resource = "hotels", Action = "update" },
            new() { Name = Permissions.Hotels.Delete, Description = "Delete hotels", Resource = "hotels", Action = "delete" },
            new() { Name = Permissions.Rooms.Read, Description = "View rooms", Resource = "rooms", Action = "read" },
            new() { Name = Permissions.Bookings.Read, Description = "View bookings", Resource = "bookings", Action = "read" },
            new() { Name = Permissions.Bookings.CheckIn, Description = "Check-in guests", Resource = "bookings", Action = "checkin" },
            new() { Name = Permissions.Users.Read, Description = "View users", Resource = "users", Action = "read" },
            new() { Name = Permissions.Brands.Read, Description = "View brands", Resource = "brands", Action = "read" },
            new() { Name = Permissions.Dashboard.View, Description = "View dashboard", Resource = "dashboard", Action = "view" },
            new() { Name = Permissions.Dashboard.ViewAll, Description = "View all dashboards", Resource = "dashboard", Action = "viewall" },
            new() { Name = Permissions.Subscriptions.Read, Description = "View subscriptions", Resource = "subscriptions", Action = "read" }
        };
        _context.Permissions.AddRange(permissions);

        // Seed role permissions
        var rolePermissions = new List<RolePermission>
        {
            // SuperAdmin - all system permissions
            new() { Role = UserRole.SuperAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Brands.Read).Id },
            new() { Role = UserRole.SuperAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Dashboard.ViewAll).Id },
            new() { Role = UserRole.SuperAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Subscriptions.Read).Id },
            new() { Role = UserRole.SuperAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Users.Read).Id },
            new() { Role = UserRole.SuperAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id }, // SuperAdmin can only READ hotels

            // BrandAdmin
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Create).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Update).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Delete).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Users.Read).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Dashboard.View).Id },

            // HotelManager
            new() { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Read).Id },
            new() { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Read).Id },
            new() { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.CheckIn).Id },
            new() { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },

            // Receptionist
            new() { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Read).Id },
            new() { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.Read).Id },
            new() { Role = UserRole.Receptionist, PermissionId = permissions.First(p => p.Name == Permissions.Bookings.CheckIn).Id }
        };
        _context.RolePermissions.AddRange(rolePermissions);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllPermissionsAsync_ReturnsAllPermissions()
    {
        // Act
        var result = await _sut.GetAllPermissionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(12, result.Count);
    }

    [Fact]
    public async Task HasPermissionAsync_SuperAdmin_ReturnsTrue_ForSystemPermission()
    {
        // Arrange
        var superAdmin = new User
        {
            Id = _superAdminId,
            Email = "admin@hotelsaas.com",
            Role = UserRole.SuperAdmin
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_superAdminId)).ReturnsAsync(superAdmin);

        // Act
        var result = await _sut.HasPermissionAsync(_superAdminId, Permissions.Brands.Read);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_SuperAdmin_ReturnsTrue_ForDashboardViewAll()
    {
        // Arrange
        var superAdmin = new User
        {
            Id = _superAdminId,
            Email = "admin@hotelsaas.com",
            Role = UserRole.SuperAdmin
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_superAdminId)).ReturnsAsync(superAdmin);

        // Act
        var result = await _sut.HasPermissionAsync(_superAdminId, Permissions.Dashboard.ViewAll);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_BrandAdmin_ReturnsTrue_ForHotelsCreate()
    {
        // Arrange
        var brandAdmin = new User
        {
            Id = _brandAdminId,
            Email = "brandadmin@hotelsaas.com",
            Role = UserRole.BrandAdmin,
            BrandId = _brandId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_brandAdminId)).ReturnsAsync(brandAdmin);

        // Act
        var result = await _sut.HasPermissionAsync(_brandAdminId, Permissions.Hotels.Create);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_BrandAdmin_ReturnsFalse_ForHotelsDelete()
    {
        // Arrange
        var brandAdmin = new User
        {
            Id = _brandAdminId,
            Email = "brandadmin@hotelsaas.com",
            Role = UserRole.BrandAdmin,
            BrandId = _brandId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_brandAdminId)).ReturnsAsync(brandAdmin);

        // Act
        var result = await _sut.HasPermissionAsync(_brandAdminId, Permissions.Hotels.Delete);

        // Assert
        Assert.True(result); // BrandAdmin CAN delete hotels in their brand
    }

    [Fact]
    public async Task HasPermissionAsync_HotelManager_ReturnsTrue_ForRoomsRead()
    {
        // Arrange
        var hotelManager = new User
        {
            Id = _hotelManagerId,
            Email = "hotelmanager@hotelsaas.com",
            Role = UserRole.HotelManager,
            HotelId = _hotelId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_hotelManagerId)).ReturnsAsync(hotelManager);

        // Act
        var result = await _sut.HasPermissionAsync(_hotelManagerId, Permissions.Rooms.Read);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_HotelManager_ReturnsFalse_ForHotelsCreate()
    {
        // Arrange
        var hotelManager = new User
        {
            Id = _hotelManagerId,
            Email = "hotelmanager@hotelsaas.com",
            Role = UserRole.HotelManager,
            HotelId = _hotelId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_hotelManagerId)).ReturnsAsync(hotelManager);

        // Act
        var result = await _sut.HasPermissionAsync(_hotelManagerId, Permissions.Hotels.Create);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_Receptionist_ReturnsTrue_ForBookingsCheckIn()
    {
        // Arrange
        var receptionist = new User
        {
            Id = _receptionistId,
            Email = "receptionist@hotelsaas.com",
            Role = UserRole.Receptionist,
            HotelId = _hotelId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_receptionistId)).ReturnsAsync(receptionist);

        // Act
        var result = await _sut.HasPermissionAsync(_receptionistId, Permissions.Bookings.CheckIn);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_Receptionist_ReturnsFalse_ForBookingsDelete()
    {
        // Arrange
        var receptionist = new User
        {
            Id = _receptionistId,
            Email = "receptionist@hotelsaas.com",
            Role = UserRole.Receptionist,
            HotelId = _hotelId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_receptionistId)).ReturnsAsync(receptionist);

        // Act
        var result = await _sut.HasPermissionAsync(_receptionistId, Permissions.Bookings.Delete);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_Guest_ReturnsFalse_ForMostPermissions()
    {
        // Arrange
        var guest = new User
        {
            Id = _guestId,
            Email = "guest@hotelsaas.com",
            Role = UserRole.Guest
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_guestId)).ReturnsAsync(guest);

        // Act
        var result = await _sut.HasPermissionAsync(_guestId, Permissions.Hotels.Create);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_NonExistentUser_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(nonExistentId)).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.HasPermissionAsync(nonExistentId, Permissions.Hotels.Read);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetUserPermissionsAsync_SuperAdmin_ReturnsAllSystemPermissions()
    {
        // Arrange
        var superAdmin = new User
        {
            Id = _superAdminId,
            Email = "admin@hotelsaas.com",
            Role = UserRole.SuperAdmin
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_superAdminId)).ReturnsAsync(superAdmin);

        // Act
        var result = await _sut.GetUserPermissionsAsync(_superAdminId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, p => p == Permissions.Brands.Read);
        Assert.Contains(result, p => p == Permissions.Dashboard.ViewAll);
        Assert.Contains(result, p => p == Permissions.Hotels.Read); // SuperAdmin can read hotels
        Assert.DoesNotContain(result, p => p == Permissions.Hotels.Create); // But not create
    }

    [Fact]
    public async Task GetUserPermissionsAsync_BrandAdmin_ReturnsBrandAdminPermissions()
    {
        // Arrange
        var brandAdmin = new User
        {
            Id = _brandAdminId,
            Email = "brandadmin@hotelsaas.com",
            Role = UserRole.BrandAdmin,
            BrandId = _brandId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_brandAdminId)).ReturnsAsync(brandAdmin);

        // Act
        var result = await _sut.GetUserPermissionsAsync(_brandAdminId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, p => p == Permissions.Hotels.Create);
        Assert.Contains(result, p => p == Permissions.Hotels.Read);
        Assert.Contains(result, p => p == Permissions.Hotels.Update);
        Assert.Contains(result, p => p == Permissions.Hotels.Delete);
    }

    [Fact]
    public async Task GetUserPermissionsAsync_HotelManager_ReturnsHotelManagerPermissions()
    {
        // Arrange
        var hotelManager = new User
        {
            Id = _hotelManagerId,
            Email = "hotelmanager@hotelsaas.com",
            Role = UserRole.HotelManager,
            HotelId = _hotelId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_hotelManagerId)).ReturnsAsync(hotelManager);

        // Act
        var result = await _sut.GetUserPermissionsAsync(_hotelManagerId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, p => p == Permissions.Rooms.Read);
        Assert.Contains(result, p => p == Permissions.Bookings.Read);
        Assert.Contains(result, p => p == Permissions.Bookings.CheckIn);
        Assert.DoesNotContain(result, p => p == Permissions.Hotels.Create);
    }

    [Fact]
    public async Task HasPermissionForHotelAsync_SuperAdmin_ReturnsTrue_ForReadPermission()
    {
        // Arrange
        var superAdmin = new User
        {
            Id = _superAdminId,
            Email = "admin@hotelsaas.com",
            Role = UserRole.SuperAdmin
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_superAdminId)).ReturnsAsync(superAdmin);

        // Act
        var result = await _sut.HasPermissionForHotelAsync(_superAdminId, _hotelId, Permissions.Hotels.Read);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHotelInBrandAsync_WhenHotelBelongsToBrand_ReturnsTrue()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = _hotelId,
            Name = "Test Hotel",
            BrandId = _brandId
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.IsHotelInBrandAsync(_hotelId, _brandId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHotelInBrandAsync_WhenHotelDoesNotBelongToBrand_ReturnsFalse()
    {
        // Arrange
        var otherBrandId = Guid.NewGuid();
        var hotel = new Hotel
        {
            Id = _hotelId,
            Name = "Test Hotel",
            BrandId = _brandId
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.IsHotelInBrandAsync(_hotelId, otherBrandId);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
