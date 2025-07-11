using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a trip in the system
    public class Trip
    {
        [Key]
        public int TripId { get; set; }

        // Foreign key to Job
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public Job? Job { get; set; }

        // Foreign key to Lorry
        public int LorryId { get; set; }
        [ForeignKey("LorryId")]
        public Lorry? Lorry { get; set; }

        // Foreign key to Driver
        public int DriverId { get; set; }
        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }

        // Foreign key to Assistant
        public int AssistantId { get; set; }
        [ForeignKey("AssistantId")]
        public Assistant? Assistant { get; set; }

        // Foreign key to Container
        public int ContainerId { get; set; }
        [ForeignKey("ContainerId")]
        public Container? Container { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "scheduled";
    }
}
