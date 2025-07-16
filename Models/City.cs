using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a city for job stops
    [Table("City")]
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string PostalCode { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string District { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Required, Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [Required, StringLength(100)]
        public string Country { get; set; } = string.Empty;
    }
}