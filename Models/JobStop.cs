using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a stop in a job route
    [Table("JobStop")]
    public class JobStop
    {
        [Key]
        public int StopId { get; set; }

        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public Job? Job { get; set; }

        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City? City { get; set; }

        [Required, StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string StopType { get; set; } = string.Empty;

        [Required]
        public int StopOrder { get; set; }

        [Required]
        public string JobStatus { get; set; } = string.Empty;
    }
}