using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobApplicationTracker.Common.Constants;

namespace JobApplicationTracker.Data.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; }

        [Required, MaxLength(AttachmentConstants.FileNameMaxLength)]
        public string FileName { get; set; } = "";

        [Required]
        public string FilePath { get; set; } = "";

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
