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
    public class Interview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public InterviewType Type { get; set; } = InterviewType.Phone;

        [MaxLength(InterviewConstants.NotesMaxLength)]
        public string? Notes { get; set; }
    }
}
