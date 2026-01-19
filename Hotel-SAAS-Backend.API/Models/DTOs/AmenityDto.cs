using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.DTOs
{
    // Request DTOs
    public class CreateAmenityDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public AmenityType Type { get; set; }
    }

    public class UpdateAmenityDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool? IsActive { get; set; }
    }

    // Response DTOs
    public class AmenityDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public AmenityType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplimentary { get; set; }
        public decimal AdditionalCost { get; set; }
    }
}
