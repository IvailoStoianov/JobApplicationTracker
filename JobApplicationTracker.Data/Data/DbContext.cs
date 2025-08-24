using JobApplicationTracker.Data.Configs;
using JobApplicationTracker.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Data
{
    public class JobApplicationTrackerDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public JobApplicationTrackerDbContext(DbContextOptions<JobApplicationTrackerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Applying Configurations
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            
            //Seeding the data
            SeedDataConfiguration.ApplySeedData(modelBuilder);

        }
        public DbSet<Job> Jobs { get; set; }
    }
}
