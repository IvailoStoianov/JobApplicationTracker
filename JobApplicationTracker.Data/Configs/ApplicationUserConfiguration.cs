using JobApplicationTracker.Common.Constants;
using JobApplicationTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Data.Configs
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Username and Email validation via Fluent API
            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(UserConstants.UserNameMaxLength);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(UserConstants.EmailMaxLength);

            // Ensure normalized fields have indexes like default Identity
            builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
        }
    }
}


