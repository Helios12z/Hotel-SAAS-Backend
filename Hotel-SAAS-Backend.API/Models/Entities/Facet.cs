namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class Facet : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Category { get; set; } // Price range, Star rating, Amenities, etc.
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<FacetValue> Values { get; set; } = new List<FacetValue>();
    }
}
