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
using RabbitMQ.Client;
using PostAPI.Models.DTOs;
using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:5242/")
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
        builder.Services.AddScoped<IPostService, PostService>();


builder.Services.AddHostedService<RabbitMQConsumerService>();

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
    }

    public class RabbitMQSettings
    {
        public string Hostname { get; set; }
        public string QueueName { get; set; }
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
            //dbContext.CleanInvalidPostsAsync().Wait();

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
                        //new Guid generation: Guid.NewGuid()
                        Id = new Guid("15db589f-d535-4180-b94b-7b3d23f67a70"), 
                        Title = "30's",
                        Description = "30's",
                        AuthorId = "auth0|656ddebb4efc549093dcfd61",
                        ImageFileName = "e5aaf056-2d6d-4471-aa32-498d07f8d2f3",
                        FileExtension = ".jpg"
                    },
                    new Post
                    {
                        Id = new Guid("1eff8b0d-6e89-49c5-9b1e-7e940368553c"),
                        Title = "70's retro",
                        Description = "Alien Isolation",
                        AuthorId = "auth0|656ddebb4efc549093dcfd61",
                        ImageFileName = "cfe1a6bf-942b-45a1-9965-fc4bfe6acd0b",
                        FileExtension = ".jpg"
                    },
                    new Post
                    {
                        Id = new Guid("99206e76-910b-46c8-a1cb-2821a6260c3b"),
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
