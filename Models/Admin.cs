using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents an administrator user in the system
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        // Foreign key to Branch
        public int? BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(100), EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string AddressLine { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required, StringLength(50)]
        public string Role { get; set; } = "BRANCH_ADMIN";

        [Column(TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}