using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a driver in the system
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;

        [StringLength(20)]
        public string? NIC { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? LicenseType { get; set; }

        public DateTime? LicenseExpiryDate { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? WorkingStatus { get; set; }
    }
}
