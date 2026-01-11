using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; } = UserRole.Guest;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string? Nationality { get; set; }
        public string? IdDocumentType { get; set; } // Passport, ID Card, Driver License
        public string? IdDocumentNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? PreferredLanguage { get; set; } = "en";
        public string? PreferredCurrency { get; set; } = "USD";
        public string? PhoneNumberVerified { get; set; }
        public string? EmailVerified { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Guid? BrandId { get; set; } // For brand staff
        public Guid? HotelId { get; set; } // For hotel staff
        public bool EmailNotificationsEnabled { get; set; } = true;
        public bool SmsNotificationsEnabled { get; set; } = false;

        // Vector embedding for personalization and recommendations
        public float[]? PreferencesEmbedding { get; set; }

        // Navigation properties
        public virtual Brand? Brand { get; set; }
        public virtual Hotel? Hotel { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
