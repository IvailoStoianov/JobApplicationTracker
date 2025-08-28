
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Data;
using Microsoft.EntityFrameworkCore;
using JobApplicationTracker.API.Infrastructure.Extentions;
using JobApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.Data.Repository;

namespace JobApplicationTracker.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Basic diagnostics for env/config loading (non-sensitive)
            var envName = builder.Environment.EnvironmentName;
            Console.WriteLine($"Environment: {envName}");


            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            // Convert postgres:// URL to Npgsql key/value if needed
            connectionString = ConvertPostgresUrlToNpgsql(connectionString);
            // Log provider-only hint to verify binding (do not print the full connection string)
            Console.WriteLine($"DB configured: {(string.IsNullOrWhiteSpace(connectionString) ? "NO" : "YES")}");
            //PostgreSQL
            builder.Services.AddDbContext<JobApplicationTracker.Data.Data.JobApplicationTrackerDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.RegisterRepositories(typeof(ApplicationUser).Assembly);
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.RegisterUserDefinedServices(typeof(IJobsService).Assembly);
            
            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter your JWT Access Token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });


            //Authentication and authorization
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!))
                };
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Apply pending migrations automatically on startup (ensures DB schema exists)
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<JobApplicationTrackerDbContext>();
                    Console.WriteLine("Applying EF Core migrations...");
                    db.Database.Migrate();
                    Console.WriteLine("EF Core migrations applied.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Migration error: {ex.Message}");
                    throw;
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static string ConvertPostgresUrlToNpgsql(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            var trimmed = input.Trim();
            if (!(trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
                  || trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
            {
                return input;
            }

            try
            {
                var uri = new Uri(trimmed);
                var userInfo = uri.UserInfo.Split(':');
                var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
                var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.Trim('/');

                // Parse query for sslmode (simple parser)
                var sslMode = "Require"; // default for Render
                var query = uri.Query; // starts with '?'
                if (!string.IsNullOrEmpty(query))
                {
                    var q = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var kv in q)
                    {
                        var parts = kv.Split('=', 2);
                        if (parts.Length == 2 && parts[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase))
                        {
                            sslMode = Uri.UnescapeDataString(parts[1]);
                        }
                    }
                }

                var kvp = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true";
                return kvp;
            }
            catch
            {
                return input;
            }
        }
    }
}
