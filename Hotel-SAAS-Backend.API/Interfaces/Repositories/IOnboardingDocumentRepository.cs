using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Interfaces.Repositories
{
    public interface IOnboardingDocumentRepository
    {
        Task<OnboardingDocument?> GetByIdAsync(Guid id);
        Task<IEnumerable<OnboardingDocument>> GetByOnboardingAsync(Guid onboardingId);
        Task<IEnumerable<OnboardingDocument>> GetByTypeAsync(Guid onboardingId, DocumentType type);
        Task<OnboardingDocument> CreateAsync(OnboardingDocument document);
        Task<OnboardingDocument> UpdateAsync(OnboardingDocument document);
        Task DeleteAsync(Guid id);
        Task<bool> HasAllRequiredDocumentsAsync(Guid onboardingId);
    }
}
