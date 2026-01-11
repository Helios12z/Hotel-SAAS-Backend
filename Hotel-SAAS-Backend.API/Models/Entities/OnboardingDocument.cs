using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    /// <summary>
    /// Documents uploaded for hotel onboarding verification
    /// </summary>
    public class OnboardingDocument : BaseEntity
    {
        public Guid OnboardingId { get; set; }
        
        public DocumentType Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; } // MIME type
        public long? FileSize { get; set; } // bytes
        
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        
        // Review
        public Guid? ReviewedById { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNotes { get; set; }
        public string? RejectionReason { get; set; }
        
        // Expiry (for licenses, certificates)
        public DateTime? ExpiryDate { get; set; }
        
        // Navigation properties
        public virtual HotelOnboarding Onboarding { get; set; } = null!;
        public virtual User? ReviewedBy { get; set; }
    }
}
