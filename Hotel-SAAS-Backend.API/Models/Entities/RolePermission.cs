using Hotel_SAAS_Backend.API.Models.Enums;

namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class RolePermission : BaseEntity
    {
        public UserRole Role { get; set; }
        public Guid PermissionId { get; set; }

        // Navigation properties
        public Permission Permission { get; set; } = null!;
    }
}
