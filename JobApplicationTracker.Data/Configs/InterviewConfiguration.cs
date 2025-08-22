using JobApplicationTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Data.Configs
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.HasOne(i => i.Job)
                   .WithMany(j => j.Interviews)
                   .HasForeignKey(i => i.JobId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.JobId);
            builder.HasIndex(i => i.Date);
        }
    }
}


