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

        // Foreign key to Job
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public Job? Job { get; set; }

        // Foreign key to City
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City? City { get; set; }

        [Required, StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public StopType StopType { get; set; } = StopType.START;

        [Required]
        public int StopOrder { get; set; }
    }

    // Types of stops for a job
    public enum StopType
    {
        START,
        INTERMEDIATE,
        DESTINATION
    }
}