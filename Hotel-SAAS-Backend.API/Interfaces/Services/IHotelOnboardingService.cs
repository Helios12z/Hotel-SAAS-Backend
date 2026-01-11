using Hotel_SAAS_Backend.API.Models.DTOs;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Services
{
    public interface IHotelOnboardingService
    {
        // Applicant operations
        Task<HotelOnboardingDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<HotelOnboardingDto>> GetMyApplicationsAsync(Guid applicantId);
        Task<HotelOnboardingDto> CreateAsync(Guid applicantId, CreateOnboardingDto dto);
        Task<HotelOnboardingDto> UpdateAsync(Guid id, Guid applicantId, UpdateOnboardingDto dto);
        Task<bool> DeleteAsync(Guid id, Guid applicantId);
        Task<HotelOnboardingDto> SubmitAsync(Guid id, Guid applicantId, SubmitOnboardingDto dto);
        
        // Document operations
        Task<OnboardingDocumentDto> UploadDocumentAsync(Guid onboardingId, Guid applicantId, UploadDocumentDto dto);
        Task<bool> DeleteDocumentAsync(Guid documentId, Guid applicantId);
        
        // Admin operations
        Task<IEnumerable<OnboardingSummaryDto>> GetAllAsync();
        Task<IEnumerable<OnboardingSummaryDto>> GetPendingReviewAsync();
        Task<PagedResultDto<OnboardingSummaryDto>> SearchAsync(
            string? query,
            OnboardingStatus? status,
            string? country,
            string? city,
            int page = 1,
            int pageSize = 20,
            string? sortBy = null,
            bool sortDescending = true);
        Task<HotelOnboardingDto> ReviewAsync(Guid id, Guid reviewerId, ReviewOnboardingDto dto);
        Task<HotelOnboardingDto> ApproveAsync(Guid id, Guid approverId);
        Task<OnboardingDocumentDto> ReviewDocumentAsync(Guid documentId, Guid reviewerId, ReviewDocumentDto dto);
        
        // Stats
        Task<OnboardingStatsDto> GetStatsAsync();
    }
}
