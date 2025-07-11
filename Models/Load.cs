using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    [Table("Load")]
    public class Load
    {
        [Key]
        public int LoadId { get; set; }

        [ForeignKey("Job")]
        public int JobId { get; set; }
        public Job? Job { get; set; }

        [ForeignKey("Trip")]
        public int? TripId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Weight { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Volume { get; set; }
    }
}