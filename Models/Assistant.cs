using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents an assistant in the system
    public class Assistant
    {
        [Key]
        public int AssistantId { get; set; }

        // Foreign key to Branch
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SecondaryPhone { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Pronouns { get; set; }
    }
}
