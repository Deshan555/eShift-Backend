using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a trip in the system
    public class Trip
    {
        [Key]
        public int TripId { get; set; }

        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public Job? Job { get; set; }

        public int? LorryId { get; set; }
        [ForeignKey("LorryId")]
        public Lorry? Lorry { get; set; }

        public int? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }

        public int? AssistantId { get; set; }
        [ForeignKey("AssistantId")]
        public Assistant? Assistant { get; set; }

        public int? ContainerId { get; set; }
        [ForeignKey("ContainerId")]
        public Container? Container { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string AdminRemark { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = "SCHEDULED";
    }
}
