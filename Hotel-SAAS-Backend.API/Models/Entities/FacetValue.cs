namespace Hotel_SAAS_Backend.API.Models.Entities
{
    public class FacetValue : BaseEntity
    {
        public Guid FacetId { get; set; }
        public string Value { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public int Count { get; set; } = 0; // For search result counts

        // Navigation properties
        public virtual Facet Facet { get; set; } = null!;
    }
}
