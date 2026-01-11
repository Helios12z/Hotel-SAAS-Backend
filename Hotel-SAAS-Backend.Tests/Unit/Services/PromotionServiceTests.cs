using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class PromotionServiceTests
{
    private readonly Mock<IPromotionRepository> _promotionRepoMock;
    private readonly PromotionService _sut;

    public PromotionServiceTests()
    {
        _promotionRepoMock = new Mock<IPromotionRepository>();
        _sut = new PromotionService(_promotionRepoMock.Object);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenCodeNotFound_ReturnsInvalid()
    {
        // Arrange
        var dto = new ValidateCouponDto 
        { 
            Code = "INVALID",
            HotelId = Guid.NewGuid(),
            BookingAmount = 100,
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("INVALID")).ReturnsAsync((Promotion?)null);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("Invalid coupon code", result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenExpired_ReturnsInvalid()
    {
        // Arrange
        var promotion = new Promotion
        {
            Code = "EXPIRED",
            Status = PromotionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(-1), // Expired yesterday
            Type = PromotionType.Percentage,
            DiscountValue = 10
        };
        var dto = new ValidateCouponDto 
        { 
            Code = "EXPIRED", 
            BookingAmount = 100,
            HotelId = Guid.NewGuid(),
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("EXPIRED")).ReturnsAsync(promotion);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("expired", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenValidPercentage_ReturnsCorrectDiscount()
    {
        // Arrange
        var promotion = new Promotion
        {
            Code = "SAVE20",
            Status = PromotionStatus.Active,
            Type = PromotionType.Percentage,
            DiscountValue = 20, // 20%
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        var dto = new ValidateCouponDto
        {
            Code = "SAVE20",
            BookingAmount = 1000,
            HotelId = Guid.NewGuid(),
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("SAVE20")).ReturnsAsync(promotion);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(200, result.CalculatedDiscount); // 20% of 1000
        Assert.Equal(800, result.FinalAmount);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenBelowMinimumAmount_ReturnsInvalid()
    {
        // Arrange
        var promotion = new Promotion
        {
            Code = "MIN500",
            Status = PromotionStatus.Active,
            Type = PromotionType.FixedAmount,
            DiscountValue = 50,
            MinBookingAmount = 500,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        var dto = new ValidateCouponDto
        {
            Code = "MIN500",
            BookingAmount = 300, // Below minimum
            HotelId = Guid.NewGuid(),
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("MIN500")).ReturnsAsync(promotion);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("minimum", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task CreateAsync_WhenCodeExists_ThrowsException()
    {
        // Arrange
        var dto = new CreatePromotionDto 
        { 
            Code = "EXISTING", 
            Name = "Test",
            Type = PromotionType.Percentage,
            DiscountValue = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        _promotionRepoMock.Setup(x => x.CodeExistsAsync("EXISTING")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenNotActive_ReturnsInvalid()
    {
        // Arrange
        var promotion = new Promotion
        {
            Code = "PAUSED",
            Status = PromotionStatus.Paused,
            Type = PromotionType.Percentage,
            DiscountValue = 10,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        var dto = new ValidateCouponDto
        {
            Code = "PAUSED",
            BookingAmount = 500,
            HotelId = Guid.NewGuid(),
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("PAUSED")).ReturnsAsync(promotion);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("not active", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenFixedAmount_ReturnsCorrectDiscount()
    {
        // Arrange
        var promotion = new Promotion
        {
            Code = "FLAT50",
            Status = PromotionStatus.Active,
            Type = PromotionType.FixedAmount,
            DiscountValue = 50,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        var dto = new ValidateCouponDto
        {
            Code = "FLAT50",
            BookingAmount = 200,
            HotelId = Guid.NewGuid(),
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("FLAT50")).ReturnsAsync(promotion);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(50, result.CalculatedDiscount);
        Assert.Equal(150, result.FinalAmount);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenMaxDiscountExceeded_CapsDiscount()
    {
        // Arrange
        var promotion = new Promotion
        {
            Code = "MAX100",
            Status = PromotionStatus.Active,
            Type = PromotionType.Percentage,
            DiscountValue = 50, // 50%
            MaxDiscountAmount = 100, // Max $100
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        var dto = new ValidateCouponDto
        {
            Code = "MAX100",
            BookingAmount = 500, // 50% = $250, but max is $100
            HotelId = Guid.NewGuid(),
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7)
        };
        _promotionRepoMock.Setup(x => x.GetByCodeAsync("MAX100")).ReturnsAsync(promotion);

        // Act
        var result = await _sut.ValidateCouponAsync(dto, Guid.NewGuid());

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(100, result.CalculatedDiscount); // Capped at 100
        Assert.Equal(400, result.FinalAmount);
    }
}
