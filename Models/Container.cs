using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a container in the system
    public class Container
    {
        [Key]
        public int ContainerId { get; set; }

        // Foreign key to Branch
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required, StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public decimal Size { get; set; }
    }
}
