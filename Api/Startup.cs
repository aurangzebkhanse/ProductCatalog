using System.Text;
using System.Text.Json;
using Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Configure services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                // Enforce authentication globally
                config.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
            });

            services.AddEndpointsApiExplorer();

            AddHealthChecks(services);

            // Configure Swagger to include JWT authentication
            ConfigureSwagger(services);

            // Register DbContext with SQLite
            RegisterDbContext(services);

            // Add api versioning
            AddApiVersioning(services);

            // Add JWT authentication
            AddAuthentication(services);

            // Add authorization policies
            AddAuthorization(services);
        }

        // Configure middleware
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Add UseRouting before UseAuthentication and UseAuthorization
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure endpoint routing
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Customize the health check response
                RegisterHealthcheckEndpoint(endpoints);
            });

            // Apply migrations
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                dbContext.Database.Migrate();
            }
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your token in the text input below.\n\nExample: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                    new string[] {}
                    }
                });
            });
        }

        private void RegisterDbContext(IServiceCollection services)
        {
            services.AddDbContext<ProductDbContext>(options =>
                    options.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));
        }

        private void AddHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                   .AddCheck("ProductCatalogApp", () => HealthCheckResult.Healthy("The application is running."))
                   .AddDbContextCheck<ProductDbContext>(); // Checks database connectivity

        }

        private void RegisterHealthcheckEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description
                        }),
                        uptime = uptime.ToString(@"dd\.hh\:mm\:ss"),
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });

        }

        private void AddApiVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = _configuration["JwtSettings:Issuer"],
                            ValidAudience = _configuration["JwtSettings:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]))
                        };
                    });
        }

        private void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ViewerPolicy", policy => policy.RequireRole("Viewer"));
            });
        }
    }
}
