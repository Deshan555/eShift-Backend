using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    [Table("Job")]
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        // Foreign key to Customer
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        // Foreign key to Branch
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RequestStatus { get; set; } = string.Empty;

        [Required]
        public DateTime? DeliveryDate { get; set; }

        [StringLength(500)]
        public string? SpecialRemark { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestContainerType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AdminApprovalStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string InvoicingStatus { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InvoicePrice { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = string.Empty;
    }
}