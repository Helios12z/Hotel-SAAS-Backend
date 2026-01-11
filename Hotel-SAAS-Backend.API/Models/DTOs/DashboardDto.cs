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
