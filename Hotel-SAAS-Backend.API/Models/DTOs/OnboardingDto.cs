using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Onboarding DTOs
    public class HotelOnboardingDto
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;
        
        // Brand
        public Guid? ExistingBrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? BrandDescription { get; set; }
        public string? BrandLogoUrl { get; set; }
        public string? BrandWebsite { get; set; }
        
        // Hotel
        public string HotelName { get; set; } = string.Empty;
        public string? HotelDescription { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int StarRating { get; set; }
        public int TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        
        // Contact
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? ContactPosition { get; set; }
        
        // Business
        public string LegalBusinessName { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        
        // Subscription
        public Guid? SelectedPlanId { get; set; }
        public string? SelectedPlanName { get; set; }
        public BillingCycle SelectedBillingCycle { get; set; }
        
        // Status
        public OnboardingStatus Status { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNotes { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        
        // Created entities
        public Guid? CreatedBrandId { get; set; }
        public Guid? CreatedHotelId { get; set; }
        public Guid? CreatedSubscriptionId { get; set; }
        
        // Documents
        public List<OnboardingDocumentDto> Documents { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OnboardingDocumentDto
    {
        public Guid Id { get; set; }
        public DocumentType Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNotes { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateOnboardingDto
    {
        // Brand Info
        public Guid? ExistingBrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? BrandDescription { get; set; }
        public string? BrandLogoUrl { get; set; }
        public string? BrandWebsite { get; set; }
        
        // Hotel Info
        public string HotelName { get; set; } = string.Empty;
        public string? HotelDescription { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int StarRating { get; set; } = 3;
        public int TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        
        // Contact Info
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? ContactPosition { get; set; }
        
        // Business Info
        public string LegalBusinessName { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        
        // Bank Info
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankRoutingNumber { get; set; }
        public string? BankSwiftCode { get; set; }
        
        // Subscription
        public Guid? SelectedPlanId { get; set; }
        public BillingCycle SelectedBillingCycle { get; set; } = BillingCycle.Monthly;
    }

    public class UpdateOnboardingDto
    {
        // Brand Info
        public string? BrandName { get; set; }
        public string? BrandDescription { get; set; }
        public string? BrandLogoUrl { get; set; }
        public string? BrandWebsite { get; set; }
        
        // Hotel Info
        public string? HotelName { get; set; }
        public string? HotelDescription { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? StarRating { get; set; }
        public int? TotalRooms { get; set; }
        public int? NumberOfFloors { get; set; }
        
        // Contact Info
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactPosition { get; set; }
        
        // Business Info
        public string? LegalBusinessName { get; set; }
        public string? TaxId { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        
        // Bank Info
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankRoutingNumber { get; set; }
        public string? BankSwiftCode { get; set; }
        
        // Subscription
        public Guid? SelectedPlanId { get; set; }
        public BillingCycle? SelectedBillingCycle { get; set; }
    }

    public class SubmitOnboardingDto
    {
        public bool AcceptedTerms { get; set; }
    }

    public class ReviewOnboardingDto
    {
        public OnboardingStatus NewStatus { get; set; }
        public string? ReviewNotes { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class UploadDocumentDto
    {
        public DocumentType Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class ReviewDocumentDto
    {
        public DocumentStatus Status { get; set; }
        public string? ReviewNotes { get; set; }
        public string? RejectionReason { get; set; }
    }

    // Summary DTOs for listing
    public class OnboardingSummaryDto
    {
        public Guid Id { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;
        public OnboardingStatus Status { get; set; }
        public int TotalRooms { get; set; }
        public int StarRating { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OnboardingStatsDto
    {
        public int TotalApplications { get; set; }
        public int DraftApplications { get; set; }
        public int PendingReview { get; set; }
        public int DocumentsRequired { get; set; }
        public int ApprovedThisMonth { get; set; }
        public int RejectedThisMonth { get; set; }
        public int ActivePartners { get; set; }
    }
}
