using JobApplicationTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Data.Configs
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasOne(j => j.User)
                   .WithMany(u => u.Jobs)
                   .HasForeignKey(j => j.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(j => j.ApplicationUserId);
            builder.HasIndex(j => j.Status);
            builder.HasIndex(j => new { j.Company, j.Position });
        }
    }
}


