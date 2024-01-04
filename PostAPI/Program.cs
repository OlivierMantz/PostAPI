using PostAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostAPI.Services;
using PostAPI.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using PostAPI.Models;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        builder.Services.AddMemoryCache();

        var secKey = builder.Configuration.GetValue<string>("Security:SecurityKey");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<IPostService, PostService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
            options.Audience = builder.Configuration["Auth0:Audience"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = "https://sublimewebapp.me/roles"
            };
        });

        var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

        var app = builder.Build();

        CreateDB(app);

        if (builder.Configuration.GetValue<bool>("RUN_MIGRATIONS_ON_STARTUP"))
        {
            ApplyMigrations(app);
        }

        SeedDatabase(app);

        app.UseCors("AllowSpecificOrigin");
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();

        var cache = app.Services.GetService<IMemoryCache>();
        cache.Set("SecurityKey", secKey);
    }

    public class RabbitMQSettings
    {
        public string Hostname { get; set; }
    }

    static void CreateDB(WebApplication app)
    {
    }

    static void ApplyMigrations(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }
    }

    static void SeedDatabase(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();

            if (!context.Posts.Any())
            {
                context.Posts.AddRange(
                    new Post
                    {
                        Id = Guid.NewGuid(), 
                        Title = "30's",
                        Description = "30's",
                        AuthorId = "auth0|656ddebb4efc549093dcfd61",
                        ImageFileName = "e5aaf056-2d6d-4471-aa32-498d07f8d2f3",
                        FileExtension = ".jpg"
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "70's retro",
                        Description = "Alien Isolation",
                        AuthorId = "auth0|656ddebb4efc549093dcfd61",
                        ImageFileName = "cfe1a6bf-942b-45a1-9965-fc4bfe6acd0b",
                        FileExtension = ".jpg"
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = ".avif image",
                        Description = "My new wallpaper",
                        AuthorId = "auth0|656ddebb4efc549093dcfd61",
                        ImageFileName = "90b1b3cd-fa1f-42eb-85d8-4ca787c3faf8",
                        FileExtension = ".avif"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
