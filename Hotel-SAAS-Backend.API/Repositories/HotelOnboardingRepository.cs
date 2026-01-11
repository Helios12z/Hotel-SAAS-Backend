using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class HotelOnboardingRepository(ApplicationDbContext context) 
        : BaseRepository<HotelOnboarding>(context), IHotelOnboardingRepository
    {
        public async Task<HotelOnboarding?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.Applicant)
                .Include(o => o.ExistingBrand)
                .Include(o => o.SelectedPlan)
                .Include(o => o.ReviewedBy)
                .Include(o => o.ApprovedBy)
                .Include(o => o.Documents)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async Task<IEnumerable<HotelOnboarding>> GetByApplicantAsync(Guid applicantId)
        {
            return await _dbSet
                .Include(o => o.SelectedPlan)
                .Include(o => o.Documents)
                .AsNoTracking()
                .Where(o => o.ApplicantId == applicantId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelOnboarding>> GetByStatusAsync(OnboardingStatus status)
        {
            return await _dbSet
                .Include(o => o.Applicant)
                .Include(o => o.SelectedPlan)
                .AsNoTracking()
                .Where(o => o.Status == status && !o.IsDeleted)
                .OrderByDescending(o => o.SubmittedAt ?? o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelOnboarding>> GetPendingReviewAsync()
        {
            return await _dbSet
                .Include(o => o.Applicant)
                .Include(o => o.SelectedPlan)
                .Include(o => o.Documents)
                .AsNoTracking()
                .Where(o => 
                    (o.Status == OnboardingStatus.Submitted || o.Status == OnboardingStatus.UnderReview) && 
                    !o.IsDeleted)
                .OrderBy(o => o.SubmittedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<HotelOnboarding> Items, int TotalCount)> SearchAsync(
            string? query,
            OnboardingStatus? status,
            string? country,
            string? city,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending)
        {
            var queryable = _dbSet
                .Include(o => o.Applicant)
                .Include(o => o.SelectedPlan)
                .AsNoTracking()
                .Where(o => !o.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                queryable = queryable.Where(o =>
                    o.HotelName.ToLower().Contains(lowerQuery) ||
                    o.BrandName.ToLower().Contains(lowerQuery) ||
                    o.ContactName.ToLower().Contains(lowerQuery) ||
                    o.ContactEmail.ToLower().Contains(lowerQuery) ||
                    o.LegalBusinessName.ToLower().Contains(lowerQuery));
            }

            if (status.HasValue)
                queryable = queryable.Where(o => o.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(country))
                queryable = queryable.Where(o => o.Country.ToLower() == country.ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                queryable = queryable.Where(o => o.City.ToLower() == city.ToLower());

            var totalCount = await queryable.CountAsync();

            queryable = sortBy?.ToLower() switch
            {
                "hotelname" => sortDescending 
                    ? queryable.OrderByDescending(o => o.HotelName) 
                    : queryable.OrderBy(o => o.HotelName),
                "submittedat" => sortDescending 
                    ? queryable.OrderByDescending(o => o.SubmittedAt) 
                    : queryable.OrderBy(o => o.SubmittedAt),
                "status" => sortDescending 
                    ? queryable.OrderByDescending(o => o.Status) 
                    : queryable.OrderBy(o => o.Status),
                _ => queryable.OrderByDescending(o => o.CreatedAt)
            };

            var items = await queryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public override async Task<HotelOnboarding?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.Applicant)
                .Include(o => o.SelectedPlan)
                .Include(o => o.Documents)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public override async Task<IEnumerable<HotelOnboarding>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.Applicant)
                .Include(o => o.SelectedPlan)
                .AsNoTracking()
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
