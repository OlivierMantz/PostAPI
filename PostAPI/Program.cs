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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using PostAPI.Models;

public class Program
{
    public static void Main(string[] args)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddMemoryCache();

        var secKey = builder.Configuration.GetValue<string>("Security:SecurityKey");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));


        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<IPostService, PostService>();


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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


        var app = builder.Build();

        CreateDB(app);

        if (builder.Configuration.GetValue<bool>("RUN_MIGRATIONS_ON_STARTUP"))
        {
            ApplyMigrations(app);
        }


        SeedDatabase(app);

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI();
        //}

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        app.Run();

        var cache = app.Services.GetService<IMemoryCache>();
        cache.Set("SecurityKey", secKey);
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

            // Check if the database is empty
            if (!context.Posts.Any())
            {
                // Seed data
                context.Posts.AddRange(
                    new Post
                    {
                        Id = 1,
                        Title = "Mountain 1",
                        Description = "abc",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e7/Everest_North_Face_toward_Base_Camp_Tibet_Luca_Galuzzi_2006.jpg/800px-Everest_North_Face_toward_Base_Camp_Tibet_Luca_Galuzzi_2006.jpg",
                        AuthorId = "1"
                    },
                    new Post
                    {
                        Id = 2,
                        Title = "Mountain 2",
                        Description = "def",
                        ImageUrl = "https://cdn.britannica.com/72/11472-050-B9734C89/Bear-Hat-Mountain-Hidden-Lake-Montana-Glacier.jpg",
                        AuthorId = "2"
                    },
                    new Post
                    {
                        Id = 3,
                        Title = "Mountain 3",
                        Description = "fgh",
                        ImageUrl = "https://www.nps.gov/common/uploads/grid_builder/mountains/crop1_1/E86EAED0-D22D-5D71-39D7716A3A19B66F.jpg?width=640&quality=90&mode=crop",
                        AuthorId = "3"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}

