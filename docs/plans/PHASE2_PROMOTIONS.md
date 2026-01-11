# Phase 2: Promotions & Coupons

## ?? M?c tiêu
H? th?ng khuy?n mãi và mã gi?m giá cho booking.

## ?? Tasks

### Task 2.1: Create Enums
**File:** `Models/Enums/PromotionType.cs`

```csharp
namespace Hotel_SAAS_Backend.API.Models.Enums
{
    public enum PromotionType
    {
        Percentage,      // Gi?m theo %
        FixedAmount,     // Gi?m s? ti?n c? ??nh
        FreeNight,       // T?ng ?êm mi?n phí (book 3 ?êm t?ng 1)
        EarlyBird,       // ??t s?m gi?m giá
        LastMinute       // ??t phút chót gi?m giá
    }

    public enum PromotionStatus
    {
        Draft,           // ?ang so?n
        Active,          // ?ang ho?t ??ng
        Paused,          // T?m d?ng
        Expired,         // H?t h?n
        Cancelled        // ?ã h?y
    }

    public enum CouponStatus
    {
        Active,
        Used,
        Expired,
        Cancelled
    }
}
```

---

### Task 2.2: Create Entities

#### File: `Models/Entities/Promotion.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Promotion : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;  // Unique code
        
        public PromotionType Type { get; set; }
        public PromotionStatus Status { get; set; } = PromotionStatus.Draft;
        
        // Discount value
        public decimal DiscountValue { get; set; }        // % ho?c s? ti?n
        public decimal? MaxDiscountAmount { get; set; }   // Gi?i h?n max discount cho %
        public decimal? MinBookingAmount { get; set; }    // ??n t?i thi?u
        
        // Validity
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Usage limits
        public int? MaxUsageCount { get; set; }           // T?ng s? l?n s? d?ng
        public int CurrentUsageCount { get; set; } = 0;
        public int? MaxUsagePerUser { get; set; }         // Gi?i h?n m?i user
        
        // Scope: null = t?t c?, có giá tr? = ch? áp d?ng cho brand/hotel ?ó
        public Guid? BrandId { get; set; }
        public Guid? HotelId { get; set; }
        
        // Conditions
        public int? MinNights { get; set; }               // S? ?êm t?i thi?u
        public int? MinDaysBeforeCheckIn { get; set; }    // ??t tr??c bao nhiêu ngày (EarlyBird)
        
        public bool IsPublic { get; set; } = true;        // Hi?n th? công khai hay private
        
        // Navigation
        public virtual Brand? Brand { get; set; }
        public virtual Hotel? Hotel { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    }
}
```

#### File: `Models/Entities/Coupon.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Coupon : BaseEntity
    {
        public Guid PromotionId { get; set; }
        public string Code { get; set; } = string.Empty;  // Unique coupon code
        
        public CouponStatus Status { get; set; } = CouponStatus.Active;
        
        // Assigned to specific user (optional)
        public Guid? AssignedToUserId { get; set; }
        
        // Usage tracking
        public Guid? UsedByUserId { get; set; }
        public Guid? UsedInBookingId { get; set; }
        public DateTime? UsedAt { get; set; }
        public decimal? DiscountApplied { get; set; }
        
        // Validity override (if different from promotion)
        public DateTime? ExpiresAt { get; set; }
        
        // Navigation
        public virtual Promotion Promotion { get; set; } = null!;
        public virtual User? AssignedToUser { get; set; }
        public virtual User? UsedByUser { get; set; }
        public virtual Booking? UsedInBooking { get; set; }
    }
}
```

#### File: `Models/Entities/UserCoupon.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.Entities
{
    // Tracking promotion usage per user (không c?n coupon riêng)
    public class UserPromotion : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PromotionId { get; set; }
        public int UsageCount { get; set; } = 0;
        public DateTime LastUsedAt { get; set; }
        
        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Promotion Promotion { get; set; } = null!;
    }
}
```

---

### Task 2.3: Update DbContext

#### File: `Data/ApplicationDbContext.cs` - Thêm DbSets và configurations
```csharp
// Thêm DbSets
public DbSet<Promotion> Promotions { get; set; }
public DbSet<Coupon> Coupons { get; set; }
public DbSet<UserPromotion> UserPromotions { get; set; }

// Trong OnModelCreating, thêm:

// Promotion Configuration
modelBuilder.Entity<Promotion>(entity =>
{
    entity.ToTable("promotions");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
    entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
    entity.Property(e => e.DiscountValue).HasPrecision(18, 2);
    entity.Property(e => e.MaxDiscountAmount).HasPrecision(18, 2);
    entity.Property(e => e.MinBookingAmount).HasPrecision(18, 2);
    entity.HasIndex(e => e.Code).IsUnique();
    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => e.StartDate);
    entity.HasIndex(e => e.EndDate);

    entity.HasOne(e => e.Brand)
        .WithMany()
        .HasForeignKey(e => e.BrandId)
        .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.Hotel)
        .WithMany()
        .HasForeignKey(e => e.HotelId)
        .OnDelete(DeleteBehavior.SetNull);
});

// Coupon Configuration
modelBuilder.Entity<Coupon>(entity =>
{
    entity.ToTable("coupons");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
    entity.Property(e => e.DiscountApplied).HasPrecision(18, 2);
    entity.HasIndex(e => e.Code).IsUnique();
    entity.HasIndex(e => e.PromotionId);
    entity.HasIndex(e => e.Status);

    entity.HasOne(e => e.Promotion)
        .WithMany(p => p.Coupons)
        .HasForeignKey(e => e.PromotionId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.AssignedToUser)
        .WithMany()
        .HasForeignKey(e => e.AssignedToUserId)
        .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.UsedByUser)
        .WithMany()
        .HasForeignKey(e => e.UsedByUserId)
        .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.UsedInBooking)
        .WithMany()
        .HasForeignKey(e => e.UsedInBookingId)
        .OnDelete(DeleteBehavior.SetNull);
});

// UserPromotion Configuration
modelBuilder.Entity<UserPromotion>(entity =>
{
    entity.ToTable("user_promotions");
    entity.HasKey(e => e.Id);
    entity.HasIndex(e => new { e.UserId, e.PromotionId }).IsUnique();

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Promotion)
        .WithMany()
        .HasForeignKey(e => e.PromotionId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

---

### Task 2.4: Create DTOs

#### File: `Models/DTOs/PromotionDto.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreatePromotionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;
        public PromotionType Type { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? HotelId { get; set; }
        public int? MinNights { get; set; }
        public int? MinDaysBeforeCheckIn { get; set; }
        public bool IsPublic { get; set; } = true;
    }

    public class UpdatePromotionDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public PromotionStatus? Status { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public int? MinNights { get; set; }
        public int? MinDaysBeforeCheckIn { get; set; }
        public bool? IsPublic { get; set; }
    }

    public class ValidateCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public Guid HotelId { get; set; }
        public decimal BookingAmount { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public class ApplyCouponDto
    {
        public string Code { get; set; } = string.Empty;
    }

    // Response DTOs
    public class PromotionDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;
        public PromotionType Type { get; set; }
        public PromotionStatus Status { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public Guid? HotelId { get; set; }
        public string? HotelName { get; set; }
        public int? MinNights { get; set; }
        public int? MinDaysBeforeCheckIn { get; set; }
        public bool IsPublic { get; set; }
        public bool IsValid { get; set; }  // Computed: còn hi?u l?c không
    }

    public class CouponDto : BaseDto
    {
        public Guid PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public CouponStatus Status { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal? DiscountApplied { get; set; }
        public DateTime? UsedAt { get; set; }
    }

    public class CouponValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Code { get; set; }
        public PromotionType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? CalculatedDiscount { get; set; }
        public decimal? FinalAmount { get; set; }
    }
}
```

---

### Task 2.5: Add Mapper Methods

#### File: `Mapping/Mapper.cs` - Thêm region m?i
```csharp
#region Promotion Mappings

public static PromotionDto ToDto(Promotion entity)
{
    var now = DateTime.UtcNow;
    return new PromotionDto
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Name = entity.Name,
        Description = entity.Description,
        Code = entity.Code,
        Type = entity.Type,
        Status = entity.Status,
        DiscountValue = entity.DiscountValue,
        MaxDiscountAmount = entity.MaxDiscountAmount,
        MinBookingAmount = entity.MinBookingAmount,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        MaxUsageCount = entity.MaxUsageCount,
        CurrentUsageCount = entity.CurrentUsageCount,
        MaxUsagePerUser = entity.MaxUsagePerUser,
        BrandId = entity.BrandId,
        BrandName = entity.Brand?.Name,
        HotelId = entity.HotelId,
        HotelName = entity.Hotel?.Name,
        MinNights = entity.MinNights,
        MinDaysBeforeCheckIn = entity.MinDaysBeforeCheckIn,
        IsPublic = entity.IsPublic,
        IsValid = entity.Status == PromotionStatus.Active && 
                  entity.StartDate <= now && 
                  entity.EndDate >= now &&
                  (entity.MaxUsageCount == null || entity.CurrentUsageCount < entity.MaxUsageCount)
    };
}

public static Promotion ToEntity(CreatePromotionDto dto)
{
    return new Promotion
    {
        Name = dto.Name,
        Description = dto.Description,
        Code = dto.Code.ToUpper(),
        Type = dto.Type,
        Status = PromotionStatus.Draft,
        DiscountValue = dto.DiscountValue,
        MaxDiscountAmount = dto.MaxDiscountAmount,
        MinBookingAmount = dto.MinBookingAmount,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        MaxUsageCount = dto.MaxUsageCount,
        MaxUsagePerUser = dto.MaxUsagePerUser,
        BrandId = dto.BrandId,
        HotelId = dto.HotelId,
        MinNights = dto.MinNights,
        MinDaysBeforeCheckIn = dto.MinDaysBeforeCheckIn,
        IsPublic = dto.IsPublic,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}

public static void UpdateEntity(UpdatePromotionDto dto, Promotion entity)
{
    if (dto.Name != null) entity.Name = dto.Name;
    if (dto.Description != null) entity.Description = dto.Description;
    if (dto.Status.HasValue) entity.Status = dto.Status.Value;
    if (dto.DiscountValue.HasValue) entity.DiscountValue = dto.DiscountValue.Value;
    if (dto.MaxDiscountAmount.HasValue) entity.MaxDiscountAmount = dto.MaxDiscountAmount.Value;
    if (dto.MinBookingAmount.HasValue) entity.MinBookingAmount = dto.MinBookingAmount.Value;
    if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
    if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
    if (dto.MaxUsageCount.HasValue) entity.MaxUsageCount = dto.MaxUsageCount.Value;
    if (dto.MaxUsagePerUser.HasValue) entity.MaxUsagePerUser = dto.MaxUsagePerUser.Value;
    if (dto.MinNights.HasValue) entity.MinNights = dto.MinNights.Value;
    if (dto.MinDaysBeforeCheckIn.HasValue) entity.MinDaysBeforeCheckIn = dto.MinDaysBeforeCheckIn.Value;
    if (dto.IsPublic.HasValue) entity.IsPublic = dto.IsPublic.Value;
    entity.UpdatedAt = DateTime.UtcNow;
}

public static CouponDto ToDto(Coupon entity)
{
    return new CouponDto
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        PromotionId = entity.PromotionId,
        PromotionName = entity.Promotion?.Name ?? "",
        Code = entity.Code,
        Status = entity.Status,
        AssignedToUserId = entity.AssignedToUserId,
        ExpiresAt = entity.ExpiresAt,
        DiscountApplied = entity.DiscountApplied,
        UsedAt = entity.UsedAt
    };
}

#endregion
```

---

### Task 2.6: Create Repository

#### File: `Interfaces/Repositories/IPromotionRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.Entities;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IPromotionRepository : IBaseRepository<Promotion>
    {
        Task<Promotion?> GetByCodeAsync(string code);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null);
        Task<IEnumerable<Promotion>> GetPublicPromotionsAsync();
        Task<bool> CodeExistsAsync(string code);
        Task IncrementUsageCountAsync(Guid promotionId);
    }
}
```

#### File: `Repositories/PromotionRepository.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class PromotionRepository(ApplicationDbContext context) 
        : BaseRepository<Promotion>(context), IPromotionRepository
    {
        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null)
        {
            var now = DateTime.UtcNow;
            var query = _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .Where(p => !p.IsDeleted && 
                            p.Status == PromotionStatus.Active &&
                            p.StartDate <= now && 
                            p.EndDate >= now);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == null || p.BrandId == brandId);

            if (hotelId.HasValue)
                query = query.Where(p => p.HotelId == null || p.HotelId == hotelId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Promotion>> GetPublicPromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Hotel)
                .AsNoTracking()
                .Where(p => !p.IsDeleted && 
                            p.IsPublic &&
                            p.Status == PromotionStatus.Active &&
                            p.StartDate <= now && 
                            p.EndDate >= now)
                .OrderByDescending(p => p.DiscountValue)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _dbSet.AnyAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
        }

        public async Task IncrementUsageCountAsync(Guid promotionId)
        {
            var promotion = await _dbSet.FindAsync(promotionId);
            if (promotion != null)
            {
                promotion.CurrentUsageCount++;
                promotion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

---

### Task 2.7: Create Service

#### File: `Interfaces/Services/IPromotionService.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IPromotionService
    {
        Task<PromotionDto?> GetByIdAsync(Guid id);
        Task<PromotionDto?> GetByCodeAsync(string code);
        Task<IEnumerable<PromotionDto>> GetAllAsync();
        Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null);
        Task<IEnumerable<PromotionDto>> GetPublicPromotionsAsync();
        Task<PromotionDto> CreateAsync(CreatePromotionDto dto);
        Task<PromotionDto> UpdateAsync(Guid id, UpdatePromotionDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ActivateAsync(Guid id);
        Task<bool> PauseAsync(Guid id);
        Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto dto, Guid userId);
        Task<decimal> CalculateDiscountAsync(string code, decimal bookingAmount, int numberOfNights);
    }
}
```

#### File: `Services/PromotionService.cs`
```csharp
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Services
{
    public class PromotionService(
        IPromotionRepository promotionRepository,
        IUserRepository userRepository) : IPromotionService
    {
        public async Task<PromotionDto?> GetByIdAsync(Guid id)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            return promotion == null ? null : Mapper.ToDto(promotion);
        }

        public async Task<PromotionDto?> GetByCodeAsync(string code)
        {
            var promotion = await promotionRepository.GetByCodeAsync(code);
            return promotion == null ? null : Mapper.ToDto(promotion);
        }

        public async Task<IEnumerable<PromotionDto>> GetAllAsync()
        {
            var promotions = await promotionRepository.GetAllAsync();
            return promotions.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync(Guid? brandId = null, Guid? hotelId = null)
        {
            var promotions = await promotionRepository.GetActivePromotionsAsync(brandId, hotelId);
            return promotions.Select(Mapper.ToDto);
        }

        public async Task<IEnumerable<PromotionDto>> GetPublicPromotionsAsync()
        {
            var promotions = await promotionRepository.GetPublicPromotionsAsync();
            return promotions.Select(Mapper.ToDto);
        }

        public async Task<PromotionDto> CreateAsync(CreatePromotionDto dto)
        {
            if (await promotionRepository.CodeExistsAsync(dto.Code))
                throw new InvalidOperationException("Promotion code already exists");

            var promotion = Mapper.ToEntity(dto);
            var created = await promotionRepository.CreateAsync(promotion);
            return Mapper.ToDto(created);
        }

        public async Task<PromotionDto> UpdateAsync(Guid id, UpdatePromotionDto dto)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException("Promotion not found");

            Mapper.UpdateEntity(dto, promotion);
            var updated = await promotionRepository.UpdateAsync(promotion);
            return Mapper.ToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await promotionRepository.DeleteAsync(id);
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            if (promotion == null) return false;

            promotion.Status = PromotionStatus.Active;
            promotion.UpdatedAt = DateTime.UtcNow;
            await promotionRepository.UpdateAsync(promotion);
            return true;
        }

        public async Task<bool> PauseAsync(Guid id)
        {
            var promotion = await promotionRepository.GetByIdAsync(id);
            if (promotion == null) return false;

            promotion.Status = PromotionStatus.Paused;
            promotion.UpdatedAt = DateTime.UtcNow;
            await promotionRepository.UpdateAsync(promotion);
            return true;
        }

        public async Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto dto, Guid userId)
        {
            var promotion = await promotionRepository.GetByCodeAsync(dto.Code);
            
            // Check if promotion exists
            if (promotion == null)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Invalid coupon code"
                };
            }

            var now = DateTime.UtcNow;
            var numberOfNights = (int)(dto.CheckOutDate - dto.CheckInDate).TotalDays;
            var daysBeforeCheckIn = (int)(dto.CheckInDate - now).TotalDays;

            // Validate promotion status and dates
            if (promotion.Status != PromotionStatus.Active)
                return InvalidResult("This promotion is not active");

            if (now < promotion.StartDate)
                return InvalidResult("This promotion has not started yet");

            if (now > promotion.EndDate)
                return InvalidResult("This promotion has expired");

            // Check usage limits
            if (promotion.MaxUsageCount.HasValue && promotion.CurrentUsageCount >= promotion.MaxUsageCount)
                return InvalidResult("This promotion has reached its usage limit");

            // Check scope (brand/hotel)
            if (promotion.HotelId.HasValue && promotion.HotelId != dto.HotelId)
                return InvalidResult("This promotion is not valid for this hotel");

            // Check minimum booking amount
            if (promotion.MinBookingAmount.HasValue && dto.BookingAmount < promotion.MinBookingAmount)
                return InvalidResult($"Minimum booking amount is {promotion.MinBookingAmount:C}");

            // Check minimum nights
            if (promotion.MinNights.HasValue && numberOfNights < promotion.MinNights)
                return InvalidResult($"Minimum stay is {promotion.MinNights} nights");

            // Check early bird condition
            if (promotion.MinDaysBeforeCheckIn.HasValue && daysBeforeCheckIn < promotion.MinDaysBeforeCheckIn)
                return InvalidResult($"Book at least {promotion.MinDaysBeforeCheckIn} days in advance");

            // Calculate discount
            var discount = CalculateDiscount(promotion, dto.BookingAmount, numberOfNights);
            var finalAmount = dto.BookingAmount - discount;

            return new CouponValidationResultDto
            {
                IsValid = true,
                Code = promotion.Code,
                DiscountType = promotion.Type,
                DiscountValue = promotion.DiscountValue,
                CalculatedDiscount = discount,
                FinalAmount = finalAmount
            };
        }

        public async Task<decimal> CalculateDiscountAsync(string code, decimal bookingAmount, int numberOfNights)
        {
            var promotion = await promotionRepository.GetByCodeAsync(code);
            if (promotion == null) return 0;

            return CalculateDiscount(promotion, bookingAmount, numberOfNights);
        }

        private static CouponValidationResultDto InvalidResult(string message)
        {
            return new CouponValidationResultDto { IsValid = false, ErrorMessage = message };
        }

        private static decimal CalculateDiscount(Models.Entities.Promotion promotion, decimal bookingAmount, int numberOfNights)
        {
            decimal discount = 0;

            switch (promotion.Type)
            {
                case PromotionType.Percentage:
                    discount = bookingAmount * (promotion.DiscountValue / 100);
                    if (promotion.MaxDiscountAmount.HasValue)
                        discount = Math.Min(discount, promotion.MaxDiscountAmount.Value);
                    break;

                case PromotionType.FixedAmount:
                    discount = promotion.DiscountValue;
                    break;

                case PromotionType.FreeNight:
                    // Example: Book 3 nights, get 1 free (DiscountValue = number of free nights per X nights)
                    var freeNights = numberOfNights / 4; // Every 4 nights, 1 free
                    discount = (bookingAmount / numberOfNights) * freeNights;
                    break;

                case PromotionType.EarlyBird:
                case PromotionType.LastMinute:
                    discount = bookingAmount * (promotion.DiscountValue / 100);
                    if (promotion.MaxDiscountAmount.HasValue)
                        discount = Math.Min(discount, promotion.MaxDiscountAmount.Value);
                    break;
            }

            return Math.Min(discount, bookingAmount); // Discount không v??t quá booking amount
        }
    }
}
```

---

### Task 2.8: Create Controller

#### File: `Controllers/PromotionsController.cs`
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
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PromotionDto>>>> GetPublicPromotions()
        {
            var promotions = await _promotionService.GetPublicPromotionsAsync();
            return Ok(new ApiResponseDto<IEnumerable<PromotionDto>>
            {
                Success = true,
                Data = promotions
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> GetById(Guid id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null)
            {
                return NotFound(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = "Promotion not found"
                });
            }

            return Ok(new ApiResponseDto<PromotionDto>
            {
                Success = true,
                Data = promotion
            });
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> GetByCode(string code)
        {
            var promotion = await _promotionService.GetByCodeAsync(code);
            if (promotion == null)
            {
                return NotFound(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = "Promotion not found"
                });
            }

            return Ok(new ApiResponseDto<PromotionDto>
            {
                Success = true,
                Data = promotion
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PromotionDto>>>> GetAll()
        {
            var promotions = await _promotionService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<PromotionDto>>
            {
                Success = true,
                Data = promotions
            });
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PromotionDto>>>> GetByHotel(Guid hotelId)
        {
            var promotions = await _promotionService.GetActivePromotionsAsync(hotelId: hotelId);
            return Ok(new ApiResponseDto<IEnumerable<PromotionDto>>
            {
                Success = true,
                Data = promotions
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> Create([FromBody] CreatePromotionDto dto)
        {
            try
            {
                var promotion = await _promotionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = promotion.Id },
                    new ApiResponseDto<PromotionDto>
                    {
                        Success = true,
                        Message = "Promotion created successfully",
                        Data = promotion
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<PromotionDto>>> Update(Guid id, [FromBody] UpdatePromotionDto dto)
        {
            try
            {
                var promotion = await _promotionService.UpdateAsync(id, dto);
                return Ok(new ApiResponseDto<PromotionDto>
                {
                    Success = true,
                    Message = "Promotion updated successfully",
                    Data = promotion
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponseDto<PromotionDto>
                {
                    Success = false,
                    Message = "Promotion not found"
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/activate")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Activate(Guid id)
        {
            var result = await _promotionService.ActivateAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Promotion activated" : "Failed to activate promotion",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpPost("{id}/pause")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Pause(Guid id)
        {
            var result = await _promotionService.PauseAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Promotion paused" : "Failed to pause promotion",
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
        {
            var result = await _promotionService.DeleteAsync(id);
            return Ok(new ApiResponseDto<bool>
            {
                Success = result,
                Message = result ? "Promotion deleted" : "Failed to delete promotion",
                Data = result
            });
        }

        [Authorize]
        [HttpPost("validate")]
        public async Task<ActionResult<ApiResponseDto<CouponValidationResultDto>>> ValidateCoupon(
            [FromBody] ValidateCouponDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _promotionService.ValidateCouponAsync(dto, userId);
            
            return Ok(new ApiResponseDto<CouponValidationResultDto>
            {
                Success = result.IsValid,
                Message = result.IsValid ? "Coupon is valid" : result.ErrorMessage,
                Data = result
            });
        }
    }
}
```

---

### Task 2.9: Register in Program.cs

```csharp
// Thêm vào Program.cs

// Repository
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();

// Service
builder.Services.AddScoped<IPromotionService, PromotionService>();
```

---

### Task 2.10: Update BookingService ?? apply coupon

#### File: `Models/DTOs/BookingDto.cs` - Update CreateBookingDto
```csharp
public class CreateBookingDto
{
    // ... existing fields ...
    public string? CouponCode { get; set; }  // Thêm field này
}
```

#### File: `Services/BookingService.cs` - Update CreateBookingAsync
```csharp
// Inject thêm IPromotionService và IPromotionRepository

public class BookingService(
    IBookingRepository bookingRepository,
    IRoomRepository roomRepository,
    IPromotionService promotionService,
    IPromotionRepository promotionRepository) : IBookingService
{
    // Update method CreateBookingAsync
    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, Guid userId)
    {
        // ... existing code for calculation ...
        
        decimal discountAmount = 0;
        string? appliedCouponCode = null;

        // Apply coupon if provided
        if (!string.IsNullOrWhiteSpace(createBookingDto.CouponCode))
        {
            var numberOfNights = (int)(createBookingDto.CheckOutDate - createBookingDto.CheckInDate).TotalDays;
            
            var validationDto = new ValidateCouponDto
            {
                Code = createBookingDto.CouponCode,
                HotelId = createBookingDto.HotelId,
                BookingAmount = calculation.Subtotal,
                CheckInDate = createBookingDto.CheckInDate,
                CheckOutDate = createBookingDto.CheckOutDate
            };

            var validationResult = await promotionService.ValidateCouponAsync(validationDto, userId);
            
            if (validationResult.IsValid && validationResult.CalculatedDiscount.HasValue)
            {
                discountAmount = validationResult.CalculatedDiscount.Value;
                appliedCouponCode = validationResult.Code;
                
                // Increment usage count
                var promotion = await promotionRepository.GetByCodeAsync(createBookingDto.CouponCode);
                if (promotion != null)
                {
                    await promotionRepository.IncrementUsageCountAsync(promotion.Id);
                }
            }
        }

        var booking = new Booking
        {
            // ... existing fields ...
            DiscountAmount = discountAmount,
            TotalAmount = calculation.TotalAmount - discountAmount,
            AppliedCouponCode = appliedCouponCode,  // Thêm field này vào Booking entity
            // ...
        };

        // ... rest of the method ...
    }
}
```

---

### Task 2.11: Create Migration

```bash
dotnet ef migrations add AddPromotionsAndCoupons
dotnet ef database update
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/PromotionServiceTests.cs`

```csharp
using Xunit;
using Moq;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class PromotionServiceTests
{
    private readonly Mock<IPromotionRepository> _promotionRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly PromotionService _sut;

    public PromotionServiceTests()
    {
        _promotionRepoMock = new Mock<IPromotionRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _sut = new PromotionService(_promotionRepoMock.Object, _userRepoMock.Object);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenCodeNotFound_ReturnsInvalid()
    {
        // Arrange
        var dto = new ValidateCouponDto { Code = "INVALID" };
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
            EndDate = DateTime.UtcNow.AddDays(-1) // Expired yesterday
        };
        var dto = new ValidateCouponDto { Code = "EXPIRED", BookingAmount = 100 };
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
        var dto = new CreatePromotionDto { Code = "EXISTING", Name = "Test" };
        _promotionRepoMock.Setup(x => x.CodeExistsAsync("EXISTING")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));
    }
}
```

---

## ? Checklist

- [ ] Enum `PromotionType`, `PromotionStatus`, `CouponStatus` ?ã t?o
- [ ] Entity `Promotion` ?ã t?o
- [ ] Entity `Coupon` ?ã t?o
- [ ] Entity `UserPromotion` ?ã t?o
- [ ] DbContext ?ã update v?i DbSets và configurations
- [ ] DTOs ?ã t?o (`PromotionDto.cs`)
- [ ] Mapper methods ?ã thêm
- [ ] `IPromotionRepository` ?ã t?o
- [ ] `PromotionRepository` ?ã implement
- [ ] `IPromotionService` ?ã t?o
- [ ] `PromotionService` ?ã implement
- [ ] `PromotionsController` ?ã t?o
- [ ] Registered trong `Program.cs`
- [ ] `CreateBookingDto` ?ã update v?i `CouponCode`
- [ ] `BookingService` ?ã update ?? apply coupon
- [ ] Migration ?ã t?o
- [ ] Unit tests ?ã vi?t
- [ ] Build passes

---

## ?? API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/promotions` | No | Get public promotions |
| GET | `/api/promotions/{id}` | No | Get promotion by ID |
| GET | `/api/promotions/code/{code}` | No | Get promotion by code |
| GET | `/api/promotions/all` | Admin | Get all promotions |
| GET | `/api/promotions/hotel/{hotelId}` | No | Get promotions for hotel |
| POST | `/api/promotions` | Admin | Create promotion |
| PUT | `/api/promotions/{id}` | Admin | Update promotion |
| POST | `/api/promotions/{id}/activate` | Admin | Activate promotion |
| POST | `/api/promotions/{id}/pause` | Admin | Pause promotion |
| DELETE | `/api/promotions/{id}` | Admin | Delete promotion |
| POST | `/api/promotions/validate` | User | Validate coupon code |
