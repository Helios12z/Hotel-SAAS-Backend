using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole? Role { get; set; }
        public UserStatus? Status { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? HotelId { get; set; }
    }

    public class UpdateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Nationality { get; set; }
        public string? IdDocumentType { get; set; }
        public string? IdDocumentNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PreferredCurrency { get; set; }
        public bool? EmailNotificationsEnabled { get; set; }
        public bool? SmsNotificationsEnabled { get; set; }
    }

    // Response DTOs
    public class UserDto : BaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public string? Nationality { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? HotelId { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
