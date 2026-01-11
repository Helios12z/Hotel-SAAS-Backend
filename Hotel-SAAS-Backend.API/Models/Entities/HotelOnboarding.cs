using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    /// <summary>
    /// Hotel onboarding application - Partners apply to list their hotels
    /// </summary>
    public class HotelOnboarding : BaseEntity
    {
        // Applicant Info
        public Guid ApplicantId { get; set; }
        
        // Brand Info (new or existing)
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
        
        // Bank Info for Payouts
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankRoutingNumber { get; set; }
        public string? BankSwiftCode { get; set; }
        
        // Subscription
        public Guid? SelectedPlanId { get; set; }
        public BillingCycle SelectedBillingCycle { get; set; } = BillingCycle.Monthly;
        
        // Status
        public OnboardingStatus Status { get; set; } = OnboardingStatus.Draft;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedById { get; set; }
        public string? ReviewNotes { get; set; }
        public string? RejectionReason { get; set; }
        
        // Approval
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        
        // Created entities after approval
        public Guid? CreatedBrandId { get; set; }
        public Guid? CreatedHotelId { get; set; }
        public Guid? CreatedSubscriptionId { get; set; }
        
        // Terms & Conditions
        public bool AcceptedTerms { get; set; } = false;
        public DateTime? AcceptedTermsAt { get; set; }
        public string? IpAddress { get; set; }
        
        // Navigation properties
        public virtual User Applicant { get; set; } = null!;
        public virtual Brand? ExistingBrand { get; set; }
        public virtual SubscriptionPlan? SelectedPlan { get; set; }
        public virtual User? ReviewedBy { get; set; }
        public virtual User? ApprovedBy { get; set; }
        public virtual ICollection<OnboardingDocument> Documents { get; set; } = new List<OnboardingDocument>();
    }
}
