# Phase 5: Dashboard & Analytics

## ?? M?c tiêu
Basic dashboard cho hotel managers v?i statistics.

## ?? Tasks

### Task 5.1: Create DTOs

#### File: `Models/DTOs/DashboardDto.cs`
```csharp
namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Revenue Statistics
    public class RevenueStatsDto
    {
        public decimal TodayRevenue { get; set; }
        public decimal WeekRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }  // % so v?i tháng tr??c
        public string Currency { get; set; } = "USD";
    }

    // Booking Statistics
    public class BookingStatsDto
    {
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CheckedInBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public decimal BookingGrowth { get; set; }  // % so v?i tháng tr??c
    }

    // Occupancy Statistics
    public class OccupancyStatsDto
    {
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int MaintenanceRooms { get; set; }
        public decimal OccupancyRate { get; set; }  // %
        public decimal AverageDailyRate { get; set; }  // ADR
        public decimal RevPAR { get; set; }  // Revenue per available room
    }

    // Review Statistics
    public class ReviewStatsDto
    {
        public int TotalReviews { get; set; }
        public int PendingReviews { get; set; }
        public float AverageRating { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
    }

    // Top Rooms
    public class TopRoomDto
    {
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // Recent Activity
    public class RecentActivityDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;  // Booking, CheckIn, CheckOut, Review
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? GuestName { get; set; }
        public string? RoomNumber { get; set; }
    }

    // Dashboard Summary
    public class DashboardSummaryDto
    {
        public RevenueStatsDto Revenue { get; set; } = new();
        public BookingStatsDto Bookings { get; set; } = new();
        public OccupancyStatsDto Occupancy { get; set; } = new();
        public ReviewStatsDto Reviews { get; set; } = new();
        public List<TopRoomDto> TopRooms { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    // Chart Data
    public class ChartDataPointDto
    {
        public string Label { get; set; } = string.Empty;  // Date or category
        public decimal Value { get; set; }
    }

    public class RevenueChartDto
    {
        public List<ChartDataPointDto> Daily { get; set; } = new();  // Last 7 days
        public List<ChartDataPointDto> Monthly { get; set; } = new();  // Last 12 months
    }

    public class BookingChartDto
    {
        public List<ChartDataPointDto> ByStatus { get; set; } = new();
        public List<ChartDataPointDto> Daily { get; set; } = new();  // Last 7 days
    }
}
```

---

### Task 5.2: Create Service Interface

#### File: `Interfaces/Services/IDashboardService.cs`
```csharp
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IDashboardService
    {
        // For specific hotel
        Task<DashboardSummaryDto> GetHotelDashboardAsync(Guid hotelId);
        Task<RevenueStatsDto> GetRevenueStatsAsync(Guid hotelId);
        Task<BookingStatsDto> GetBookingStatsAsync(Guid hotelId);
        Task<OccupancyStatsDto> GetOccupancyStatsAsync(Guid hotelId);
        Task<ReviewStatsDto> GetReviewStatsAsync(Guid hotelId);
        Task<List<TopRoomDto>> GetTopRoomsAsync(Guid hotelId, int count = 5);
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync(Guid hotelId, int count = 10);
        Task<RevenueChartDto> GetRevenueChartAsync(Guid hotelId);
        Task<BookingChartDto> GetBookingChartAsync(Guid hotelId);
        
        // For brand (aggregate of all hotels)
        Task<DashboardSummaryDto> GetBrandDashboardAsync(Guid brandId);
    }
}
```

---

### Task 5.3: Create Service Implementation

#### File: `Services/DashboardService.cs`
```csharp
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services
{
    public class DashboardService(ApplicationDbContext context) : IDashboardService
    {
        public async Task<DashboardSummaryDto> GetHotelDashboardAsync(Guid hotelId)
        {
            return new DashboardSummaryDto
            {
                Revenue = await GetRevenueStatsAsync(hotelId),
                Bookings = await GetBookingStatsAsync(hotelId),
                Occupancy = await GetOccupancyStatsAsync(hotelId),
                Reviews = await GetReviewStatsAsync(hotelId),
                TopRooms = await GetTopRoomsAsync(hotelId),
                RecentActivities = await GetRecentActivitiesAsync(hotelId)
            };
        }

        public async Task<RevenueStatsDto> GetRevenueStatsAsync(Guid hotelId)
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);

            var completedBookings = context.Bookings
                .Where(b => b.HotelId == hotelId && 
                            !b.IsDeleted &&
                            (b.Status == BookingStatus.CheckedOut || b.Status == BookingStatus.Confirmed));

            var todayRevenue = await completedBookings
                .Where(b => b.CheckOutDate.Date == today)
                .SumAsync(b => b.TotalAmount);

            var weekRevenue = await completedBookings
                .Where(b => b.CheckOutDate >= weekStart && b.CheckOutDate < today.AddDays(1))
                .SumAsync(b => b.TotalAmount);

            var monthRevenue = await completedBookings
                .Where(b => b.CheckOutDate >= monthStart && b.CheckOutDate < today.AddDays(1))
                .SumAsync(b => b.TotalAmount);

            var yearRevenue = await completedBookings
                .Where(b => b.CheckOutDate >= yearStart && b.CheckOutDate < today.AddDays(1))
                .SumAsync(b => b.TotalAmount);

            var lastMonthRevenue = await completedBookings
                .Where(b => b.CheckOutDate >= lastMonthStart && b.CheckOutDate <= lastMonthEnd)
                .SumAsync(b => b.TotalAmount);

            var growth = lastMonthRevenue > 0 
                ? ((monthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 
                : 0;

            return new RevenueStatsDto
            {
                TodayRevenue = todayRevenue,
                WeekRevenue = weekRevenue,
                MonthRevenue = monthRevenue,
                YearRevenue = yearRevenue,
                RevenueGrowth = growth
            };
        }

        public async Task<BookingStatsDto> GetBookingStatsAsync(Guid hotelId)
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);

            var bookings = context.Bookings
                .Where(b => b.HotelId == hotelId && !b.IsDeleted);

            var totalBookings = await bookings.CountAsync();
            var pendingBookings = await bookings.CountAsync(b => b.Status == BookingStatus.Pending);
            var confirmedBookings = await bookings.CountAsync(b => b.Status == BookingStatus.Confirmed);
            var checkedInBookings = await bookings.CountAsync(b => b.Status == BookingStatus.CheckedIn);
            var cancelledBookings = await bookings.CountAsync(b => b.Status == BookingStatus.Cancelled);

            var todayCheckIns = await bookings
                .CountAsync(b => b.CheckInDate.Date == today && 
                                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn));

            var todayCheckOuts = await bookings
                .CountAsync(b => b.CheckOutDate.Date == today && b.Status == BookingStatus.CheckedIn);

            var thisMonthBookings = await bookings
                .CountAsync(b => b.CreatedAt >= monthStart);

            var lastMonthBookings = await bookings
                .CountAsync(b => b.CreatedAt >= lastMonthStart && b.CreatedAt <= lastMonthEnd);

            var growth = lastMonthBookings > 0 
                ? ((decimal)(thisMonthBookings - lastMonthBookings) / lastMonthBookings) * 100 
                : 0;

            return new BookingStatsDto
            {
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings,
                ConfirmedBookings = confirmedBookings,
                CheckedInBookings = checkedInBookings,
                CancelledBookings = cancelledBookings,
                TodayCheckIns = todayCheckIns,
                TodayCheckOuts = todayCheckOuts,
                BookingGrowth = growth
            };
        }

        public async Task<OccupancyStatsDto> GetOccupancyStatsAsync(Guid hotelId)
        {
            var rooms = context.Rooms
                .Where(r => r.HotelId == hotelId && !r.IsDeleted);

            var totalRooms = await rooms.CountAsync();
            var occupiedRooms = await rooms.CountAsync(r => r.Status == RoomStatus.Occupied);
            var availableRooms = await rooms.CountAsync(r => r.Status == RoomStatus.Available);
            var maintenanceRooms = await rooms.CountAsync(r => r.Status == RoomStatus.Maintenance);

            var occupancyRate = totalRooms > 0 ? ((decimal)occupiedRooms / totalRooms) * 100 : 0;

            // Calculate ADR (Average Daily Rate) from recent bookings
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var monthlyBookings = await context.BookingRooms
                .Include(br => br.Booking)
                .Where(br => br.Booking.HotelId == hotelId && 
                            !br.Booking.IsDeleted &&
                            br.Booking.CheckInDate >= monthStart)
                .ToListAsync();

            var adr = monthlyBookings.Count > 0 
                ? monthlyBookings.Average(br => br.Price) 
                : await rooms.AverageAsync(r => r.BasePrice);

            var revPar = adr * (occupancyRate / 100);

            return new OccupancyStatsDto
            {
                TotalRooms = totalRooms,
                OccupiedRooms = occupiedRooms,
                AvailableRooms = availableRooms,
                MaintenanceRooms = maintenanceRooms,
                OccupancyRate = Math.Round(occupancyRate, 2),
                AverageDailyRate = Math.Round(adr, 2),
                RevPAR = Math.Round(revPar, 2)
            };
        }

        public async Task<ReviewStatsDto> GetReviewStatsAsync(Guid hotelId)
        {
            var reviews = context.Reviews
                .Where(r => r.HotelId == hotelId && !r.IsDeleted);

            var totalReviews = await reviews.CountAsync();
            var pendingReviews = await reviews.CountAsync(r => r.Status == ReviewStatus.Pending);
            var approvedReviews = reviews.Where(r => r.Status == ReviewStatus.Approved);

            var averageRating = totalReviews > 0 
                ? (float)await approvedReviews.AverageAsync(r => r.Rating) 
                : 0;

            var fiveStarCount = await approvedReviews.CountAsync(r => r.Rating == 5);
            var fourStarCount = await approvedReviews.CountAsync(r => r.Rating == 4);
            var threeStarCount = await approvedReviews.CountAsync(r => r.Rating == 3);
            var twoStarCount = await approvedReviews.CountAsync(r => r.Rating == 2);
            var oneStarCount = await approvedReviews.CountAsync(r => r.Rating == 1);

            return new ReviewStatsDto
            {
                TotalReviews = totalReviews,
                PendingReviews = pendingReviews,
                AverageRating = (float)Math.Round(averageRating, 1),
                FiveStarCount = fiveStarCount,
                FourStarCount = fourStarCount,
                ThreeStarCount = threeStarCount,
                TwoStarCount = twoStarCount,
                OneStarCount = oneStarCount
            };
        }

        public async Task<List<TopRoomDto>> GetTopRoomsAsync(Guid hotelId, int count = 5)
        {
            var topRooms = await context.BookingRooms
                .Include(br => br.Room)
                .Include(br => br.Booking)
                .Where(br => br.Room.HotelId == hotelId && !br.Booking.IsDeleted)
                .GroupBy(br => new { br.RoomId, br.Room.RoomNumber, br.Room.Type })
                .Select(g => new TopRoomDto
                {
                    RoomId = g.Key.RoomId,
                    RoomNumber = g.Key.RoomNumber,
                    RoomType = g.Key.Type.ToString(),
                    BookingCount = g.Count(),
                    TotalRevenue = g.Sum(br => br.Price)
                })
                .OrderByDescending(r => r.TotalRevenue)
                .Take(count)
                .ToListAsync();

            return topRooms;
        }

        public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync(Guid hotelId, int count = 10)
        {
            var activities = new List<RecentActivityDto>();

            // Recent bookings
            var recentBookings = await context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();

            foreach (var booking in recentBookings)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = booking.Id,
                    Type = "Booking",
                    Description = $"New booking {booking.ConfirmationNumber}",
                    Timestamp = booking.CreatedAt,
                    GuestName = booking.GuestName,
                    RoomNumber = booking.BookingRooms.FirstOrDefault()?.Room?.RoomNumber
                });
            }

            // Recent check-ins
            var recentCheckIns = await context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.HotelId == hotelId && 
                           !b.IsDeleted && 
                           b.CheckedInAt != null)
                .OrderByDescending(b => b.CheckedInAt)
                .Take(5)
                .ToListAsync();

            foreach (var booking in recentCheckIns)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = booking.Id,
                    Type = "CheckIn",
                    Description = $"Guest checked in",
                    Timestamp = booking.CheckedInAt!.Value,
                    GuestName = booking.GuestName,
                    RoomNumber = booking.BookingRooms.FirstOrDefault()?.Room?.RoomNumber
                });
            }

            // Recent reviews
            var recentReviews = await context.Reviews
                .Include(r => r.Guest)
                .Where(r => r.HotelId == hotelId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            foreach (var review in recentReviews)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = review.Id,
                    Type = "Review",
                    Description = $"New {review.Rating}-star review",
                    Timestamp = review.CreatedAt,
                    GuestName = $"{review.Guest.FirstName} {review.Guest.LastName}"
                });
            }

            return activities
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToList();
        }

        public async Task<RevenueChartDto> GetRevenueChartAsync(Guid hotelId)
        {
            var today = DateTime.UtcNow.Date;
            var chart = new RevenueChartDto();

            // Daily revenue (last 7 days)
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var revenue = await context.Bookings
                    .Where(b => b.HotelId == hotelId && 
                               !b.IsDeleted &&
                               b.CheckOutDate.Date == date &&
                               (b.Status == BookingStatus.CheckedOut || b.Status == BookingStatus.Confirmed))
                    .SumAsync(b => b.TotalAmount);

                chart.Daily.Add(new ChartDataPointDto
                {
                    Label = date.ToString("MMM dd"),
                    Value = revenue
                });
            }

            // Monthly revenue (last 12 months)
            for (int i = 11; i >= 0; i--)
            {
                var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var revenue = await context.Bookings
                    .Where(b => b.HotelId == hotelId && 
                               !b.IsDeleted &&
                               b.CheckOutDate >= monthStart &&
                               b.CheckOutDate <= monthEnd &&
                               (b.Status == BookingStatus.CheckedOut || b.Status == BookingStatus.Confirmed))
                    .SumAsync(b => b.TotalAmount);

                chart.Monthly.Add(new ChartDataPointDto
                {
                    Label = monthStart.ToString("MMM yyyy"),
                    Value = revenue
                });
            }

            return chart;
        }

        public async Task<BookingChartDto> GetBookingChartAsync(Guid hotelId)
        {
            var today = DateTime.UtcNow.Date;
            var chart = new BookingChartDto();

            // By status
            var statusCounts = await context.Bookings
                .Where(b => b.HotelId == hotelId && !b.IsDeleted)
                .GroupBy(b => b.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in statusCounts)
            {
                chart.ByStatus.Add(new ChartDataPointDto
                {
                    Label = item.Status.ToString(),
                    Value = item.Count
                });
            }

            // Daily bookings (last 7 days)
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = await context.Bookings
                    .Where(b => b.HotelId == hotelId && 
                               !b.IsDeleted &&
                               b.CreatedAt.Date == date)
                    .CountAsync();

                chart.Daily.Add(new ChartDataPointDto
                {
                    Label = date.ToString("MMM dd"),
                    Value = count
                });
            }

            return chart;
        }

        public async Task<DashboardSummaryDto> GetBrandDashboardAsync(Guid brandId)
        {
            // Get all hotel IDs for the brand
            var hotelIds = await context.Hotels
                .Where(h => h.BrandId == brandId && !h.IsDeleted)
                .Select(h => h.Id)
                .ToListAsync();

            if (!hotelIds.Any())
            {
                return new DashboardSummaryDto();
            }

            // Aggregate stats from all hotels
            var summary = new DashboardSummaryDto();
            
            foreach (var hotelId in hotelIds)
            {
                var hotelRevenue = await GetRevenueStatsAsync(hotelId);
                summary.Revenue.TodayRevenue += hotelRevenue.TodayRevenue;
                summary.Revenue.WeekRevenue += hotelRevenue.WeekRevenue;
                summary.Revenue.MonthRevenue += hotelRevenue.MonthRevenue;
                summary.Revenue.YearRevenue += hotelRevenue.YearRevenue;

                var hotelBookings = await GetBookingStatsAsync(hotelId);
                summary.Bookings.TotalBookings += hotelBookings.TotalBookings;
                summary.Bookings.PendingBookings += hotelBookings.PendingBookings;
                summary.Bookings.ConfirmedBookings += hotelBookings.ConfirmedBookings;
                summary.Bookings.CheckedInBookings += hotelBookings.CheckedInBookings;
                summary.Bookings.CancelledBookings += hotelBookings.CancelledBookings;
                summary.Bookings.TodayCheckIns += hotelBookings.TodayCheckIns;
                summary.Bookings.TodayCheckOuts += hotelBookings.TodayCheckOuts;

                var hotelOccupancy = await GetOccupancyStatsAsync(hotelId);
                summary.Occupancy.TotalRooms += hotelOccupancy.TotalRooms;
                summary.Occupancy.OccupiedRooms += hotelOccupancy.OccupiedRooms;
                summary.Occupancy.AvailableRooms += hotelOccupancy.AvailableRooms;
                summary.Occupancy.MaintenanceRooms += hotelOccupancy.MaintenanceRooms;
            }

            // Calculate aggregate occupancy rate
            if (summary.Occupancy.TotalRooms > 0)
            {
                summary.Occupancy.OccupancyRate = Math.Round(
                    ((decimal)summary.Occupancy.OccupiedRooms / summary.Occupancy.TotalRooms) * 100, 2);
            }

            return summary;
        }
    }
}
```

---

### Task 5.4: Create Controller

#### File: `Controllers/DashboardController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Models.DTOs;

namespace Hotel_SAAS_Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,BrandAdmin,HotelManager")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<ApiResponseDto<DashboardSummaryDto>>> GetHotelDashboard(Guid hotelId)
        {
            var result = await _dashboardService.GetHotelDashboardAsync(hotelId);
            return Ok(new ApiResponseDto<DashboardSummaryDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/revenue")]
        public async Task<ActionResult<ApiResponseDto<RevenueStatsDto>>> GetRevenueStats(Guid hotelId)
        {
            var result = await _dashboardService.GetRevenueStatsAsync(hotelId);
            return Ok(new ApiResponseDto<RevenueStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/bookings")]
        public async Task<ActionResult<ApiResponseDto<BookingStatsDto>>> GetBookingStats(Guid hotelId)
        {
            var result = await _dashboardService.GetBookingStatsAsync(hotelId);
            return Ok(new ApiResponseDto<BookingStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/occupancy")]
        public async Task<ActionResult<ApiResponseDto<OccupancyStatsDto>>> GetOccupancyStats(Guid hotelId)
        {
            var result = await _dashboardService.GetOccupancyStatsAsync(hotelId);
            return Ok(new ApiResponseDto<OccupancyStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/reviews")]
        public async Task<ActionResult<ApiResponseDto<ReviewStatsDto>>> GetReviewStats(Guid hotelId)
        {
            var result = await _dashboardService.GetReviewStatsAsync(hotelId);
            return Ok(new ApiResponseDto<ReviewStatsDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/top-rooms")]
        public async Task<ActionResult<ApiResponseDto<List<TopRoomDto>>>> GetTopRooms(
            Guid hotelId,
            [FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetTopRoomsAsync(hotelId, count);
            return Ok(new ApiResponseDto<List<TopRoomDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/activities")]
        public async Task<ActionResult<ApiResponseDto<List<RecentActivityDto>>>> GetRecentActivities(
            Guid hotelId,
            [FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetRecentActivitiesAsync(hotelId, count);
            return Ok(new ApiResponseDto<List<RecentActivityDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/charts/revenue")]
        public async Task<ActionResult<ApiResponseDto<RevenueChartDto>>> GetRevenueChart(Guid hotelId)
        {
            var result = await _dashboardService.GetRevenueChartAsync(hotelId);
            return Ok(new ApiResponseDto<RevenueChartDto>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("hotel/{hotelId}/charts/bookings")]
        public async Task<ActionResult<ApiResponseDto<BookingChartDto>>> GetBookingChart(Guid hotelId)
        {
            var result = await _dashboardService.GetBookingChartAsync(hotelId);
            return Ok(new ApiResponseDto<BookingChartDto>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize(Roles = "SuperAdmin,BrandAdmin")]
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<ApiResponseDto<DashboardSummaryDto>>> GetBrandDashboard(Guid brandId)
        {
            var result = await _dashboardService.GetBrandDashboardAsync(brandId);
            return Ok(new ApiResponseDto<DashboardSummaryDto>
            {
                Success = true,
                Data = result
            });
        }
    }
}
```

---

### Task 5.5: Register in Program.cs

```csharp
// Service
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

---

## ?? Unit Tests

### File: `Hotel-SAAS-Backend.Tests/Unit/Services/DashboardServiceTests.cs`

```csharp
using Xunit;
using Microsoft.EntityFrameworkCore;
using Hotel_SAAS_Backend.API.Services;
using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.Tests.Unit.Services;

public class DashboardServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DashboardService _sut;
    private readonly Guid _hotelId;

    public DashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new DashboardService(_context);
        _hotelId = Guid.NewGuid();

        SeedTestData();
    }

    private void SeedTestData()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Test Brand" };
        var hotel = new Hotel { Id = _hotelId, Name = "Test Hotel", BrandId = brand.Id, Brand = brand };
        
        var room1 = new Room 
        { 
            Id = Guid.NewGuid(), 
            HotelId = _hotelId, 
            RoomNumber = "101", 
            Status = RoomStatus.Available,
            BasePrice = 100
        };
        var room2 = new Room 
        { 
            Id = Guid.NewGuid(), 
            HotelId = _hotelId, 
            RoomNumber = "102", 
            Status = RoomStatus.Occupied,
            BasePrice = 150
        };

        var guest = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@test.com", 
            FirstName = "Test", 
            LastName = "Guest",
            PasswordHash = "hash"
        };

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = _hotelId,
            Hotel = hotel,
            GuestId = guest.Id,
            Guest = guest,
            ConfirmationNumber = "BK123",
            CheckInDate = DateTime.UtcNow.Date,
            CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
            TotalAmount = 300,
            Status = BookingStatus.Confirmed
        };

        _context.Brands.Add(brand);
        _context.Hotels.Add(hotel);
        _context.Rooms.AddRange(room1, room2);
        _context.Users.Add(guest);
        _context.Bookings.Add(booking);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetBookingStatsAsync_ReturnsCorrectStats()
    {
        // Act
        var result = await _sut.GetBookingStatsAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalBookings);
        Assert.Equal(1, result.ConfirmedBookings);
    }

    [Fact]
    public async Task GetOccupancyStatsAsync_ReturnsCorrectStats()
    {
        // Act
        var result = await _sut.GetOccupancyStatsAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRooms);
        Assert.Equal(1, result.OccupiedRooms);
        Assert.Equal(1, result.AvailableRooms);
        Assert.Equal(50, result.OccupancyRate); // 1/2 = 50%
    }

    [Fact]
    public async Task GetHotelDashboardAsync_ReturnsFullSummary()
    {
        // Act
        var result = await _sut.GetHotelDashboardAsync(_hotelId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Revenue);
        Assert.NotNull(result.Bookings);
        Assert.NotNull(result.Occupancy);
        Assert.NotNull(result.Reviews);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

---

## ? Checklist

- [ ] DTOs ?ã t?o (`DashboardDto.cs`)
- [ ] `IDashboardService` ?ã t?o
- [ ] `DashboardService` ?ã implement
- [ ] `DashboardController` ?ã t?o
- [ ] Registered trong `Program.cs`
- [ ] Unit tests ?ã vi?t
- [ ] Build passes

---

## ?? API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/dashboard/hotel/{hotelId}` | Manager+ | Full dashboard |
| GET | `/api/dashboard/hotel/{hotelId}/revenue` | Manager+ | Revenue stats |
| GET | `/api/dashboard/hotel/{hotelId}/bookings` | Manager+ | Booking stats |
| GET | `/api/dashboard/hotel/{hotelId}/occupancy` | Manager+ | Occupancy stats |
| GET | `/api/dashboard/hotel/{hotelId}/reviews` | Manager+ | Review stats |
| GET | `/api/dashboard/hotel/{hotelId}/top-rooms` | Manager+ | Top performing rooms |
| GET | `/api/dashboard/hotel/{hotelId}/activities` | Manager+ | Recent activities |
| GET | `/api/dashboard/hotel/{hotelId}/charts/revenue` | Manager+ | Revenue chart data |
| GET | `/api/dashboard/hotel/{hotelId}/charts/bookings` | Manager+ | Booking chart data |
| GET | `/api/dashboard/brand/{brandId}` | BrandAdmin+ | Brand-wide dashboard |
