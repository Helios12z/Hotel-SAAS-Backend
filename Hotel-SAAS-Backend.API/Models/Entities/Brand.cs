using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; } = true;
        public string? TaxId { get; set; }
        public string? BusinessLicense { get; set; }
        public string? CommissionRate { get; set; } = "15"; // Percentage
        public string? PaymentTerms { get; set; }
        public string? ContractStart { get; set; }
        public string? ContractEnd { get; set; }

        // Navigation properties
        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
