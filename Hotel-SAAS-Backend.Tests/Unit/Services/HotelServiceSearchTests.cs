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
    public async Task SearchHotelsAdvancedAsync_WithValidRequest_ReturnsPagedResult()
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
            new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "Hanoi Hotel",
                Brand = new Brand { Name = "Test Brand" },
                Rooms = new List<Room>()
            }
        };

        _hotelRepositoryMock
            .Setup(x => x.SearchWithPaginationAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<List<Guid>?>(), It.IsAny<float?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .ReturnsAsync((hotels.AsEnumerable(), 1));

        // Act
        var result = await _sut.SearchHotelsAdvancedAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task SearchHotelsAdvancedAsync_WithNoResults_ReturnsEmptyPagedResult()
    {
        // Arrange
        var request = new HotelSearchRequestDto { Query = "NonExistent", Page = 1, PageSize = 10 };

        _hotelRepositoryMock
            .Setup(x => x.SearchWithPaginationAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<List<Guid>?>(), It.IsAny<float?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .ReturnsAsync((new List<Hotel>().AsEnumerable(), 0));

        // Act
        var result = await _sut.SearchHotelsAdvancedAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task SearchHotelsAdvancedAsync_WithFilters_PassesCorrectParameters()
    {
        // Arrange
        var request = new HotelSearchRequestDto
        {
            Query = "Test",
            City = "Hanoi",
            Country = "Vietnam",
            MinStarRating = 3,
            MaxStarRating = 5,
            MinPrice = 50,
            MaxPrice = 200,
            MinRating = 4.0f,
            Page = 2,
            PageSize = 20,
            SortBy = "price",
            SortDescending = true
        };

        _hotelRepositoryMock
            .Setup(x => x.SearchWithPaginationAsync(
                "Test", "Hanoi", "Vietnam",
                3, 5, 50m, 200m,
                It.IsAny<List<Guid>?>(), 4.0f,
                2, 20, "price", true))
            .ReturnsAsync((new List<Hotel>().AsEnumerable(), 0));

        // Act
        await _sut.SearchHotelsAdvancedAsync(request);

        // Assert
        _hotelRepositoryMock.Verify(x => x.SearchWithPaginationAsync(
            "Test", "Hanoi", "Vietnam",
            3, 5, 50m, 200m,
            It.IsAny<List<Guid>?>(), 4.0f,
            2, 20, "price", true), Times.Once);
    }

    [Fact]
    public async Task SearchHotelsAdvancedAsync_CalculatesPaginationCorrectly()
    {
        // Arrange
        var request = new HotelSearchRequestDto
        {
            Page = 2,
            PageSize = 10
        };

        var hotels = Enumerable.Range(1, 10).Select(i => new Hotel
        {
            Id = Guid.NewGuid(),
            Name = $"Hotel {i}",
            Brand = new Brand { Name = "Brand" },
            Rooms = new List<Room>()
        }).ToList();

        _hotelRepositoryMock
            .Setup(x => x.SearchWithPaginationAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<List<Guid>?>(), It.IsAny<float?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .ReturnsAsync((hotels.AsEnumerable(), 25)); // Total 25 items

        // Act
        var result = await _sut.SearchHotelsAdvancedAsync(request);

        // Assert
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages); // 25 / 10 = 2.5, ceiling = 3
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }
}
