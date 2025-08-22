using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Extentions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedDataFromJson<TEntity>(this ModelBuilder modelBuilder, string filePath) where TEntity : class
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(baseDirectory, filePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(string.Format("Seed file not found at: {0}", fullPath));
            }

            var jsonData = File.ReadAllText(fullPath);
            var data = JsonConvert.DeserializeObject<List<TEntity>>(jsonData);
            if (data != null)
            {
                modelBuilder.Entity<TEntity>().HasData(data);
            }
        }
    }
}
