using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Hotel_SAAS_Backend.API.Authorization;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Models.Constants;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.Tests.Unit.Authorization;

public class PermissionHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Guid _superAdminId = Guid.NewGuid();
    private readonly Guid _brandAdminId = Guid.NewGuid();
    private readonly Guid _hotelManagerId = Guid.NewGuid();
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly Guid _hotelId = Guid.NewGuid();

    public PermissionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts => opts.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped(_ => _context);
        services.AddHttpContextAccessor();
        _serviceProvider = services.BuildServiceProvider();

        SeedTestData();
    }

    private void SeedTestData()
    {
        var permissions = new List<Permission>
        {
            new() { Name = Permissions.Hotels.Read, Resource = "hotels", Action = "read" },
            new() { Name = Permissions.Hotels.Create, Resource = "hotels", Action = "create" },
            new() { Name = Permissions.Hotels.Update, Resource = "hotels", Action = "update" },
            new() { Name = Permissions.Hotels.Delete, Resource = "hotels", Action = "delete" },
            new() { Name = Permissions.Rooms.Read, Resource = "rooms", Action = "read" },
            new() { Name = Permissions.Bookings.Read, Resource = "bookings", Action = "read" }
        };
        _context.Permissions.AddRange(permissions);

        var rolePermissions = new List<RolePermission>
        {
            new() { Role = UserRole.SuperAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Create).Id },
            new() { Role = UserRole.BrandAdmin, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
            new() { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Hotels.Read).Id },
            new() { Role = UserRole.HotelManager, PermissionId = permissions.First(p => p.Name == Permissions.Rooms.Read).Id }
        };
        _context.RolePermissions.AddRange(rolePermissions);

        var hotel = new Hotel { Id = _hotelId, Name = "Test Hotel", BrandId = _brandId };
        _context.Hotels.Add(hotel);

        _context.SaveChanges();
    }

    private HttpContext CreateHttpContext(Guid userId, UserRole role, string permission, Guid? brandId = null, Guid? hotelId = null, Guid? routeHotelId = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim("permissions", permission),
                new Claim("scope", role == UserRole.SuperAdmin ? "system" :
                                   role == UserRole.BrandAdmin ? "brand" : "hotel")
            })
        });

        if (brandId.HasValue)
            httpContext.User.AddClaim(new Claim("brandId", brandId.Value.ToString()));
        if (hotelId.HasValue)
            httpContext.User.AddClaim(new Claim("hotelId", hotelId.Value.ToString()));

        if (routeHotelId.HasValue)
        {
            httpContext.Request.RouteValues["hotelId"] = routeHotelId.Value.ToString();
        }

        return httpContext;
    }

    [Fact]
    public async Task HandleRequirementAsync_SuperAdmin_CanReadHotels_Succeeds()
    {
        // Arrange
        var httpContext = CreateHttpContext(_superAdminId, UserRole.SuperAdmin, Permissions.Hotels.Read);
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permissions.Hotels.Read) },
            httpContext.User,
            null);

        var handler = new PermissionHandler(_serviceProvider, _httpContextAccessorMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_SuperAdmin_CannotModifyHotels_Fails()
    {
        // Arrange
        var httpContext = CreateHttpContext(_superAdminId, UserRole.SuperAdmin, Permissions.Hotels.Create);
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permissions.Hotels.Create) },
            httpContext.User,
            httpContext);

        var handler = new PermissionHandler(_serviceProvider, _httpContextAccessorMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_BrandAdmin_CanCreateHotels_Succeeds()
    {
        // Arrange
        var httpContext = CreateHttpContext(_brandAdminId, UserRole.BrandAdmin, Permissions.Hotels.Create, _brandId, null, _hotelId);
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permissions.Hotels.Create) },
            httpContext.User,
            httpContext);

        var handler = new PermissionHandler(_serviceProvider, _httpContextAccessorMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_HotelManager_CannotCreateHotels_Fails()
    {
        // Arrange
        var httpContext = CreateHttpContext(_hotelManagerId, UserRole.HotelManager, Permissions.Hotels.Create, null, _hotelId, _hotelId);
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permissions.Hotels.Create) },
            httpContext.User,
            httpContext);

        var handler = new PermissionHandler(_serviceProvider, _httpContextAccessorMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_HotelManager_CanReadRooms_Succeeds()
    {
        // Arrange
        var httpContext = CreateHttpContext(_hotelManagerId, UserRole.HotelManager, Permissions.Rooms.Read, null, _hotelId);
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permissions.Rooms.Read) },
            httpContext.User,
            httpContext);

        var handler = new PermissionHandler(_serviceProvider, _httpContextAccessorMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_NoUserId_Fails()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, UserRole.Guest.ToString())
            })
        });
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permissions.Hotels.Read) },
            httpContext.User,
            httpContext);

        var handler = new PermissionHandler(_serviceProvider, _httpContextAccessorMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
