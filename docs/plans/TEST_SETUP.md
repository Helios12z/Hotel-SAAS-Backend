# Test Project Setup Guide

## ?? C?u trúc Test Project

```
Hotel-SAAS-Backend.Tests/
??? Hotel-SAAS-Backend.Tests.csproj
??? Unit/
?   ??? Services/
?   ?   ??? HotelServiceSearchTests.cs
?   ?   ??? RoomAvailabilityTests.cs
?   ?   ??? PromotionServiceTests.cs
?   ?   ??? WishlistServiceTests.cs
?   ?   ??? NotificationServiceTests.cs
?   ?   ??? DashboardServiceTests.cs
?   ?   ??? GuestProfileServiceTests.cs
?   ??? Mapping/
?       ??? MapperTests.cs
??? Integration/
?   ??? (optional for MVP)
??? Fixtures/
    ??? TestDbContextFixture.cs
    ??? TestDataBuilder.cs
```

## ?? T?o Test Project

### 1. T?o project
```bash
cd Hotel-SAAS-Backend
dotnet new xunit -n Hotel-SAAS-Backend.Tests
dotnet sln add Hotel-SAAS-Backend.Tests/Hotel-SAAS-Backend.Tests.csproj
```

### 2. Thêm references và packages

#### File: `Hotel-SAAS-Backend.Tests/Hotel-SAAS-Backend.Tests.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="FluentAssertions" Version="6.12.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Hotel-SAAS-Backend.API\Hotel-SAAS-Backend.API.csproj" />
  </ItemGroup>

</Project>
```

### 3. Cài ??t packages
```bash
cd Hotel-SAAS-Backend.Tests
dotnet restore
```

---

## ?? Test Fixtures

### File: `Fixtures/TestDbContextFixture.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.Tests.Fixtures;

public class TestDbContextFixture : IDisposable
{
    public ApplicationDbContext Context { get; }

    public TestDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
```

### File: `Fixtures/TestDataBuilder.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Fixtures;

public static class TestDataBuilder
{
    public static Brand CreateBrand(string name = "Test Brand")
    {
        return new Brand
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Hotel CreateHotel(Guid brandId, string name = "Test Hotel")
    {
        return new Hotel
        {
            Id = Guid.NewGuid(),
            BrandId = brandId,
            Name = name,
            City = "Test City",
            Country = "Test Country",
            StarRating = 4,
            IsActive = true,
            IsVerified = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Room CreateRoom(Guid hotelId, string roomNumber = "101", decimal basePrice = 100)
    {
        return new Room
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            RoomNumber = roomNumber,
            BasePrice = basePrice,
            MaxOccupancy = 2,
            Type = RoomType.Standard,
            BedType = BedType.Queen,
            Status = RoomStatus.Available,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static User CreateUser(string email = "test@test.com", UserRole role = UserRole.Guest)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hashed_password",
            Role = role,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Booking CreateBooking(
        Guid hotelId, 
        Guid guestId, 
        DateTime? checkIn = null, 
        DateTime? checkOut = null,
        BookingStatus status = BookingStatus.Confirmed)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            GuestId = guestId,
            ConfirmationNumber = "BK" + Guid.NewGuid().ToString()[..8].ToUpper(),
            CheckInDate = checkIn ?? DateTime.UtcNow.AddDays(7),
            CheckOutDate = checkOut ?? DateTime.UtcNow.AddDays(10),
            NumberOfGuests = 2,
            NumberOfRooms = 1,
            TotalAmount = 300,
            Currency = "USD",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Promotion CreatePromotion(
        string code = "TEST20",
        PromotionType type = PromotionType.Percentage,
        decimal discountValue = 20)
    {
        return new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "Test Promotion",
            Code = code,
            Type = type,
            DiscountValue = discountValue,
            Status = PromotionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30),
            IsPublic = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Review CreateReview(
        Guid hotelId,
        Guid guestId,
        int rating = 5,
        ReviewStatus status = ReviewStatus.Approved)
    {
        return new Review
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            GuestId = guestId,
            Rating = rating,
            Title = "Great stay",
            Comment = "Really enjoyed my stay here.",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
```

---

## ?? Test Naming Convention

```
MethodName_Scenario_ExpectedResult
```

**Ví d?:**
- `GetByIdAsync_WhenExists_ReturnsDto`
- `CreateBookingAsync_WithValidData_ReturnsBookingDto`
- `ValidateCouponAsync_WhenExpired_ReturnsInvalid`
- `CalculateDiscount_WithPercentage_ReturnsCorrectAmount`

---

## ?? Test Template

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.Tests.Fixtures;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class ExampleServiceTests
{
    private readonly Mock<IExampleRepository> _repositoryMock;
    private readonly ExampleService _sut; // System Under Test

    public ExampleServiceTests()
    {
        _repositoryMock = new Mock<IExampleRepository>();
        _sut = new ExampleService(_repositoryMock.Object);
    }

    [Fact]
    public async Task MethodName_Scenario_ExpectedResult()
    {
        // Arrange
        var expected = new ExampleDto { Id = Guid.NewGuid() };
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Example());

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        // ho?c
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(100, 20, 20)]   // 20% of 100 = 20
    [InlineData(500, 20, 100)]  // 20% of 500 = 100
    [InlineData(50, 20, 10)]    // 20% of 50 = 10
    public void CalculateDiscount_WithDifferentAmounts_ReturnsCorrectDiscount(
        decimal amount, decimal percentage, decimal expectedDiscount)
    {
        // Arrange & Act
        var result = _sut.CalculatePercentageDiscount(amount, percentage);

        // Assert
        result.Should().Be(expectedDiscount);
    }
}
```

---

## ?? Ch?y Tests

```bash
# Ch?y t?t c? tests
dotnet test

# Ch?y v?i verbose output
dotnet test --verbosity normal

# Ch?y specific test class
dotnet test --filter "FullyQualifiedName~PromotionServiceTests"

# Ch?y specific test method
dotnet test --filter "FullyQualifiedName~ValidateCouponAsync_WhenCodeNotFound"

# Ch?y v?i code coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## ? Minimum Test Coverage per Phase

| Phase | Required Tests |
|-------|---------------|
| Phase 1: Search | 3-4 tests |
| Phase 2: Promotions | 4-5 tests |
| Phase 3: Wishlist | 3-4 tests |
| Phase 4: Notifications | 3-4 tests |
| Phase 5: Dashboard | 3-4 tests |
| Phase 6: Guest Profile | 3-4 tests |

**T?i thi?u cho m?i service:**
1. Happy path test (thành công)
2. Not found test (entity không t?n t?i)
3. Validation error test (d? li?u không h?p l?)

---

## ?? Tips

1. **S? d?ng InMemory Database** cho tests c?n DbContext th?t
2. **S? d?ng Moq** cho unit tests ??n thu?n
3. **Không test private methods** - test qua public interface
4. **M?i test ch? test 1 behavior**
5. **Tên test ph?i mô t? rõ scenario và expected result**
