using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IHotelOnboardingRepository
    {
        Task<HotelOnboarding?> GetByIdAsync(Guid id);
        Task<HotelOnboarding?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<HotelOnboarding>> GetAllAsync();
        Task<IEnumerable<HotelOnboarding>> GetByApplicantAsync(Guid applicantId);
        Task<IEnumerable<HotelOnboarding>> GetByStatusAsync(OnboardingStatus status);
        Task<IEnumerable<HotelOnboarding>> GetPendingReviewAsync();
        Task<HotelOnboarding> CreateAsync(HotelOnboarding onboarding);
        Task<HotelOnboarding> UpdateAsync(HotelOnboarding onboarding);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<(IEnumerable<HotelOnboarding> Items, int TotalCount)> SearchAsync(
            string? query,
            OnboardingStatus? status,
            string? country,
            string? city,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending);
    }
}
