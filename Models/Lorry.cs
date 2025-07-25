using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a lorry (truck) in the system
    public class Lorry
    {
        [Key]
        public int LorryId { get; set; }

        // Foreign key to Branch
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required, StringLength(50)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        public decimal Capacity { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "available";

        [StringLength(100)]
        public string? VehicleModel { get; set; }

        [StringLength(50)]
        public string? VehicleColor { get; set; }

        [StringLength(100)]
        public string? InsuranceNumber { get; set; }

        public DateTime? InsuranceExpiryDate { get; set; }
        [StringLength(100)]

        public string? TransportCompany { get; set; }

    }
}
