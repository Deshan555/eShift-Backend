using System.ComponentModel.DataAnnotations;

namespace eShift.Models
{
    // Represents a branch office
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;
    }
}