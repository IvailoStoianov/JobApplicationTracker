using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobApplicationTracker.Common.Enums;
using JobApplicationTracker.Common.Constants;

namespace JobApplicationTracker.Data.Models
{
    public class Job
    {
        public Job()
        {
            this.Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser User { get; set; }

        [Required, MaxLength(JobConstants.CompanyMaxLength)]
        public string Company { get; set; } = "";

        [Required, MaxLength(JobConstants.PositionMaxLength)]
        public string Position { get; set; } = "";

        [Required]
        public JobStatus Status { get; set; } = JobStatus.Applied;

        [Required]
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [MaxLength(JobConstants.NotesMaxLength)]
        public string? Notes { get; set; }
    }
}
