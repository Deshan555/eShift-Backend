using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Represents a customer
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string EmailAddress { get; set; } = string.Empty;

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ActiveStatus { get; set; } = string.Empty;
    }
}