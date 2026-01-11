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
    public async Task IsInWishlistAsync_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        _wishlistRepoMock.Setup(x => x.ExistsAsync(userId, hotelId)).ReturnsAsync(false);

        // Act
        var result = await _sut.IsInWishlistAsync(userId, hotelId);

        // Assert
        Assert.False(result);
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

    [Fact]
    public async Task RemoveFromWishlistAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var wishlist = new Wishlist { Id = Guid.NewGuid(), UserId = userId, HotelId = hotelId };

        _wishlistRepoMock.Setup(x => x.GetByUserAndHotelAsync(userId, hotelId)).ReturnsAsync(wishlist);
        _wishlistRepoMock.Setup(x => x.DeleteAsync(wishlist.Id)).ReturnsAsync(true);

        // Act
        var result = await _sut.RemoveFromWishlistAsync(userId, hotelId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveFromWishlistAsync_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();

        _wishlistRepoMock.Setup(x => x.GetByUserAndHotelAsync(userId, hotelId)).ReturnsAsync((Wishlist?)null);

        // Act
        var result = await _sut.RemoveFromWishlistAsync(userId, hotelId);

        // Assert
        Assert.False(result);
    }
}
