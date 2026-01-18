using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System;

namespace TaskTrackerAPI.Infrastructure
{
    public static class SetUpInfrastructure
    {
        public static void AddInfrastructure(this IServiceCollection services) 
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task API",
                    Version = "v1",
                    Description = "A simple task management API with EF Core InMemory database"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalSpa",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TaskDb"));
        }
    }
}
