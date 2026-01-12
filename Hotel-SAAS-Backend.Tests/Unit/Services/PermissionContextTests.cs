using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Hotel_SAAS_Backend.API.Services;
using System.Security.Claims;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class PermissionContextTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Guid _superAdminId = Guid.NewGuid();
    private readonly Guid _brandAdminId = Guid.NewGuid();
    private readonly Guid _hotelManagerId = Guid.NewGuid();
    private readonly Guid _receptionistId = Guid.NewGuid();
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly Guid _hotelId = Guid.NewGuid();

    public PermissionContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _userRepositoryMock = new Mock<IUserRepository>();

        // Setup default mock for http context
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    }

    private PermissionContext CreatePermissionContextWithUser(UserRole role, Guid? brandId = null, Guid? hotelId = null)
    {
        var user = new User
        {
            Id = role == UserRole.SuperAdmin ? _superAdminId :
                 role == UserRole.BrandAdmin ? _brandAdminId :
                 role == UserRole.HotelManager ? _hotelManagerId : _receptionistId,
            Email = $"test{role}@hotelsaas.com",
            FirstName = "Test",
            LastName = "User",
            Role = role,
            BrandId = brandId,
            HotelId = hotelId
        };

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim("scope", role == UserRole.SuperAdmin ? "system" :
                                   role == UserRole.BrandAdmin ? "brand" : "hotel"),
                new Claim("permissions", "")
            })
        });

        if (brandId.HasValue)
            httpContext.User.AddClaim(new Claim("brandId", brandId.Value.ToString()));
        if (hotelId.HasValue)
            httpContext.User.AddClaim(new Claim("hotelId", hotelId.Value.ToString()));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        return new PermissionContext(_httpContextAccessorMock.Object, null!, _userRepositoryMock.Object, _context);
    }

    [Fact]
    public void IsSuperAdmin_WhenRoleIsSuperAdmin_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.SuperAdmin);

        // Act & Assert
        Assert.True(context.IsSuperAdmin);
    }

    [Fact]
    public void IsSuperAdmin_WhenRoleIsBrandAdmin_ReturnsFalse()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin);

        // Act & Assert
        Assert.False(context.IsSuperAdmin);
    }

    [Fact]
    public void IsBrandAdmin_WhenRoleIsBrandAdmin_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.True(context.IsBrandAdmin);
    }

    [Fact]
    public void IsHotelManager_WhenRoleIsHotelManager_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.HotelManager, null, _hotelId);

        // Act & Assert
        Assert.True(context.IsHotelManager);
    }

    [Fact]
    public void IsReceptionist_WhenRoleIsReceptionist_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.Receptionist, null, _hotelId);

        // Act & Assert
        Assert.True(context.IsReceptionist);
    }

    [Fact]
    public void Role_ReturnsCorrectRole()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.Equal(UserRole.BrandAdmin, context.Role);
    }

    [Fact]
    public void BrandId_ReturnsCorrectBrandId()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.Equal(_brandId, context.BrandId);
    }

    [Fact]
    public void HotelId_ReturnsCorrectHotelId()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.HotelManager, null, _hotelId);

        // Act & Assert
        Assert.Equal(_hotelId, context.HotelId);
    }

    [Fact]
    public void BrandId_ReturnsNull_WhenNoBrand()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.SuperAdmin);

        // Act & Assert
        Assert.Null(context.BrandId);
    }

    [Fact]
    public void HotelId_ReturnsNull_WhenNoHotel()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.Null(context.HotelId);
    }

    [Fact]
    public void GetCurrentUser_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = _brandAdminId,
            Email = "brandadmin@hotelsaas.com",
            Role = UserRole.BrandAdmin,
            BrandId = _brandId
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(_brandAdminId)).ReturnsAsync(expectedUser);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("userId", _brandAdminId.ToString()),
                new Claim(ClaimTypes.Role, UserRole.BrandAdmin.ToString())
            })
        });
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var context = new PermissionContext(_httpContextAccessorMock.Object, null!, _userRepositoryMock.Object, _context);

        // Act
        var result = context.GetCurrentUser();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_brandAdminId, result.Id);
        Assert.Equal(UserRole.BrandAdmin, result.Role);
    }

    [Fact]
    public void GetCurrentUser_WhenNoUser_ReturnsNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("userId", Guid.Empty.ToString())
            })
        });
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var context = new PermissionContext(_httpContextAccessorMock.Object, null!, _userRepositoryMock.Object, _context);

        // Act
        var result = context.GetCurrentUser();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CanCreateHotelsAsync_WhenBrandAdminWithBrand_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.True(context.CanCreateHotelsAsync().Result);
    }

    [Fact]
    public void CanCreateHotelsAsync_WhenBrandAdminWithoutBrand_ReturnsFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("userId", _brandAdminId.ToString()),
                new Claim(ClaimTypes.Role, UserRole.BrandAdmin.ToString())
            })
        });
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var context = new PermissionContext(_httpContextAccessorMock.Object, null!, _userRepositoryMock.Object, _context);

        // Act & Assert
        Assert.False(context.CanCreateHotelsAsync().Result);
    }

    [Fact]
    public void CanCreateHotelsAsync_WhenHotelManager_ReturnsFalse()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.HotelManager, null, _hotelId);

        // Act & Assert
        Assert.False(context.CanCreateHotelsAsync().Result);
    }

    [Fact]
    public void CanDeleteHotelsAsync_WhenBrandAdminWithBrand_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.True(context.CanDeleteHotelsAsync().Result);
    }

    [Fact]
    public void CanDeleteHotelsAsync_WhenHotelManager_ReturnsFalse()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.HotelManager, null, _hotelId);

        // Act & Assert
        Assert.False(context.CanDeleteHotelsAsync().Result);
    }

    [Fact]
    public void CanManageHotelsInBrandAsync_WhenBrandAdminOwnBrand_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);

        // Act & Assert
        Assert.True(context.CanManageHotelsInBrandAsync(_brandId).Result);
    }

    [Fact]
    public void CanManageHotelsInBrandAsync_WhenBrandAdminOtherBrand_ReturnsFalse()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.BrandAdmin, _brandId);
        var otherBrandId = Guid.NewGuid();

        // Act & Assert
        Assert.False(context.CanManageHotelsInBrandAsync(otherBrandId).Result);
    }

    [Fact]
    public void CanManageHotelsInBrandAsync_WhenSuperAdmin_ReturnsTrue()
    {
        // Arrange
        var context = CreatePermissionContextWithUser(UserRole.SuperAdmin);

        // Act & Assert
        Assert.True(context.CanManageHotelsInBrandAsync(_brandId).Result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
