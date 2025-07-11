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

        [StringLength(100)]
        public string? State { get; set; }

        [Required, StringLength(100)]
        public string Country { get; set; } = string.Empty;
    }
}