using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class BookingRoom : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public decimal Price { get; set; }
        public int NumberOfAdults { get; set; } = 1;
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfInfants { get; set; } = 0;
        public string? GuestName { get; set; }
        public string? SpecialRequests { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}
