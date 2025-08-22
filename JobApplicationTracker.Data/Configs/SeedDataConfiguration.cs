using JobApplicationTracker.Data.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Configs
{
    public static class SeedDataConfiguration
    {
        public static void ApplySeedData(ModelBuilder modelBuilder)
        {
            //modelBuilder.SeedDataFromJson<Category>(CategoriesSeedPath);
            
        }
    }
}
