using JobApplicationTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Data.Configs
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasOne(a => a.Job)
                   .WithMany(j => j.Attachments)
                   .HasForeignKey(a => a.JobId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.JobId);
            builder.HasIndex(a => new { a.JobId, a.FileName });
        }
    }
}


