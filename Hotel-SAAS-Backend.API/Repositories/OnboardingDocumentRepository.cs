using Hotel_SAAS_Backend.API.Data;
using Hotel_SAAS_Backend.API.Interfaces.Repositories;
using Hotel_SAAS_Backend.API.Models.Entities;
using Hotel_SAAS_Backend.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hotel_SAAS_Backend.API.Repositories
{
    public class OnboardingDocumentRepository(ApplicationDbContext context) 
        : BaseRepository<OnboardingDocument>(context), IOnboardingDocumentRepository
    {
        public async Task<IEnumerable<OnboardingDocument>> GetByOnboardingAsync(Guid onboardingId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.OnboardingId == onboardingId && !d.IsDeleted)
                .OrderBy(d => d.Type)
                .ThenByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<OnboardingDocument>> GetByTypeAsync(Guid onboardingId, DocumentType type)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.OnboardingId == onboardingId && d.Type == type && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasAllRequiredDocumentsAsync(Guid onboardingId)
        {
            var requiredTypes = new[] 
            { 
                DocumentType.BusinessLicense, 
                DocumentType.TaxCertificate,
                DocumentType.OwnerIdCard
            };

            var uploadedTypes = await _dbSet
                .Where(d => d.OnboardingId == onboardingId && !d.IsDeleted && d.Status == DocumentStatus.Approved)
                .Select(d => d.Type)
                .Distinct()
                .ToListAsync();

            return requiredTypes.All(rt => uploadedTypes.Contains(rt));
        }

        public override async Task<OnboardingDocument?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.ReviewedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }
    }
}
