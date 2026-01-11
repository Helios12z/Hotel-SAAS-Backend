using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class HotelRepository(ApplicationDbContext context) : BaseRepository<Hotel>(context), IHotelRepository
    {
        public async Task<Hotel?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(h => h.Brand)
                .Include(h => h.Images)
                .Include(h => h.Amenities)
                    .ThenInclude(ha => ha.Amenity)
                .Include(h => h.Reviews
                    .Where(r => r.Status.ToString() == "Approved"))
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        }

        public async Task<IEnumerable<Hotel>> GetByBrandAsync(Guid brandId)
        {
            return await _dbSet
                .Include(h => h.Brand)
                .AsNoTracking()
                .Where(h => h.BrandId == brandId && !h.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Hotel>> GetByCityAsync(string city)
        {
            return await _dbSet
                .Include(h => h.Brand)
                .AsNoTracking()
                .Where(h => h.City == city && !h.IsDeleted && h.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Hotel>> SearchAsync(string query, string? city = null, int? starRating = null)
        {
            var hotels = _dbSet
                .Include(h => h.Brand)
                .AsNoTracking()
                .Where(h => !h.IsDeleted && h.IsActive);

            if (!string.IsNullOrWhiteSpace(query))
            {
                hotels = hotels.Where(h =>
                    h.Name.Contains(query) ||
                    h.Description != null && h.Description.Contains(query) ||
                    h.City != null && h.City.Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                hotels = hotels.Where(h => h.City == city);
            }

            if (starRating.HasValue)
            {
                hotels = hotels.Where(h => h.StarRating == starRating.Value);
            }

            return await hotels.ToListAsync();
        }

        public async Task UpdateRatingAsync(Guid hotelId, float newRating)
        {
            var hotel = await _dbSet.FindAsync(hotelId);
            if (hotel != null)
            {
                hotel.AverageRating = newRating;
                hotel.ReviewCount = await _context.Reviews
                    .CountAsync(r => r.HotelId == hotelId && r.Status.ToString() == "Approved");
                hotel.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public override async Task<Hotel?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(h => h.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        }

        public override async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            return await _dbSet
                .Include(h => h.Brand)
                .AsNoTracking()
                .Where(h => !h.IsDeleted)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Hotel> Hotels, int TotalCount)> SearchWithPaginationAsync(
            string? query,
            string? city,
            string? country,
            int? minStarRating,
            int? maxStarRating,
            decimal? minPrice,
            decimal? maxPrice,
            List<Guid>? amenityIds,
            float? minRating,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending)
        {
            var hotels = _dbSet
                .Include(h => h.Brand)
                .Include(h => h.Rooms)
                .Include(h => h.Amenities)
                    .ThenInclude(ha => ha.Amenity)
                .AsNoTracking()
                .Where(h => !h.IsDeleted && h.IsActive);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                hotels = hotels.Where(h =>
                    h.Name.ToLower().Contains(lowerQuery) ||
                    (h.Description != null && h.Description.ToLower().Contains(lowerQuery)) ||
                    (h.City != null && h.City.ToLower().Contains(lowerQuery)));
            }

            if (!string.IsNullOrWhiteSpace(city))
                hotels = hotels.Where(h => h.City != null && h.City.ToLower() == city.ToLower());

            if (!string.IsNullOrWhiteSpace(country))
                hotels = hotels.Where(h => h.Country != null && h.Country.ToLower() == country.ToLower());

            if (minStarRating.HasValue)
                hotels = hotels.Where(h => h.StarRating >= minStarRating.Value);

            if (maxStarRating.HasValue)
                hotels = hotels.Where(h => h.StarRating <= maxStarRating.Value);

            if (minPrice.HasValue)
                hotels = hotels.Where(h => h.Rooms.Any(r => r.BasePrice >= minPrice.Value));

            if (maxPrice.HasValue)
                hotels = hotels.Where(h => h.Rooms.Any(r => r.BasePrice <= maxPrice.Value));

            if (minRating.HasValue)
                hotels = hotels.Where(h => h.AverageRating >= minRating.Value);

            if (amenityIds != null && amenityIds.Count > 0)
                hotels = hotels.Where(h => h.Amenities.Any(a => amenityIds.Contains(a.AmenityId)));

            // Get total count before pagination
            var totalCount = await hotels.CountAsync();

            // Apply sorting
            hotels = sortBy?.ToLower() switch
            {
                "price" => sortDescending
                    ? hotels.OrderByDescending(h => h.Rooms.Min(r => r.BasePrice))
                    : hotels.OrderBy(h => h.Rooms.Min(r => r.BasePrice)),
                "rating" => sortDescending
                    ? hotels.OrderByDescending(h => h.AverageRating)
                    : hotels.OrderBy(h => h.AverageRating),
                "name" => sortDescending
                    ? hotels.OrderByDescending(h => h.Name)
                    : hotels.OrderBy(h => h.Name),
                "star" => sortDescending
                    ? hotels.OrderByDescending(h => h.StarRating)
                    : hotels.OrderBy(h => h.StarRating),
                _ => hotels.OrderByDescending(h => h.AverageRating) // Default sort
            };

            // Apply pagination
            var result = await hotels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (result, totalCount);
        }
    }
}
