using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a driver in the system
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        // Foreign key to Branch
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;
    }
}
