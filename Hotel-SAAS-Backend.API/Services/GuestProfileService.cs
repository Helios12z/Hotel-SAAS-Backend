using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Interfaces.Services;
using Hotel_SAAS_Backend.API.Mapping;
using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Services
{
    public class GuestProfileService(
        IUserRepository userRepository,
        IRecentlyViewedRepository recentlyViewedRepository,
        ApplicationDbContext context) : IGuestProfileService
    {
        public async Task<GuestProfileDto?> GetProfileAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var totalBookings = await context.Bookings
                .CountAsync(b => b.GuestId == userId && !b.IsDeleted);

            var completedStays = await context.Bookings
                .CountAsync(b => b.GuestId == userId &&
                                !b.IsDeleted &&
                                b.Status == BookingStatus.CheckedOut);

            var totalReviews = await context.Reviews
                .CountAsync(r => r.GuestId == userId && !r.IsDeleted);

            return Mapper.ToGuestProfileDto(user, totalBookings, completedStays, totalReviews);
        }

        public async Task<GuestProfileDto?> UpdatePreferencesAsync(Guid userId, UpdateGuestPreferencesDto dto)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            Mapper.UpdateGuestPreferences(dto, user);
            await userRepository.UpdateAsync(user);

            return await GetProfileAsync(userId);
        }

        public async Task<GuestBookingHistoryDto> GetBookingHistoryAsync(Guid userId, BookingHistoryFilterDto filter)
        {
            var query = context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.Guest)
                .Where(b => b.GuestId == userId && !b.IsDeleted);

            // Apply filters
            if (filter.Status.HasValue)
                query = query.Where(b => b.Status == filter.Status.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(b => b.CheckInDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(b => b.CheckOutDate <= filter.ToDate.Value);

            if (filter.HotelId.HasValue)
                query = query.Where(b => b.HotelId == filter.HotelId.Value);

            var totalCount = await query.CountAsync();

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "date" => filter.SortDescending
                    ? query.OrderByDescending(b => b.CheckInDate)
                    : query.OrderBy(b => b.CheckInDate),
                "amount" => filter.SortDescending
                    ? query.OrderByDescending(b => b.TotalAmount)
                    : query.OrderBy(b => b.TotalAmount),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            // Pagination
            var bookings = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var items = bookings.Select(Mapper.ToDto).ToList();

            // Calculate stats
            var allBookings = await context.Bookings
                .Include(b => b.Hotel)
                .Where(b => b.GuestId == userId && !b.IsDeleted)
                .ToListAsync();

            var stats = new BookingStatsSummaryDto
            {
                TotalBookings = allBookings.Count,
                CompletedBookings = allBookings.Count(b => b.Status == BookingStatus.CheckedOut),
                CancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled),
                UpcomingBookings = allBookings.Count(b =>
                    (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                    b.CheckInDate > DateTime.UtcNow),
                TotalSpent = allBookings
                    .Where(b => b.Status == BookingStatus.CheckedOut)
                    .Sum(b => b.TotalAmount),
                MostVisitedCity = allBookings
                    .Where(b => b.Hotel?.City != null)
                    .GroupBy(b => b.Hotel!.City)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key
            };

            return new GuestBookingHistoryDto
            {
                Bookings = new PagedResultDto<BookingDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                },
                Stats = stats
            };
        }

        public async Task<IEnumerable<RecentlyViewedHotelDto>> GetRecentlyViewedAsync(Guid userId, int limit = 10)
        {
            var recentlyViewed = await recentlyViewedRepository.GetByUserIdAsync(userId, limit);
            return recentlyViewed.Select(Mapper.ToDto);
        }

        public async Task TrackHotelViewAsync(Guid userId, Guid hotelId)
        {
            await recentlyViewedRepository.AddOrUpdateViewAsync(userId, hotelId);
        }

        public async Task ClearRecentlyViewedAsync(Guid userId)
        {
            await recentlyViewedRepository.ClearUserHistoryAsync(userId);
        }
    }
}
