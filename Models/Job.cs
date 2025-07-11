using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShift.Models
{
    // Status for a job
    public enum JobStatus
    {
        PENDING,
        IN_PROGRESS,
        COMPLETED
    }

    // Represents a job
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
        public JobStatus Status { get; set; } = JobStatus.PENDING;
    }
}