namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class UserHotelPermission : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid HotelId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsGranted { get; set; } = true;

        // Navigation properties
        public Permission Permission { get; set; } = null!;
    }
}
