using System;
using System.Linq;
using PostAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PostAPI.Data;

public static class PrepDb
{
    // for testing
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<PostContext>());
        }
    }

    private static void SeedData(PostContext context)
    {
        if (!context.Post.Any())
        {
            Console.WriteLine("--> Seeding data...");

            context.Post.AddRange(
                new Post()
                {
                    Id = Guid.NewGuid(),
                    Title = "Some picture",
                    Description = "#hashtag",
                    AuthorId = "1",
                    ImageFileName = "test image",
                },
                new Post()
                {
                    Id = Guid.NewGuid(),
                    Title = "Second Ansel Adams ",
                    Description = "#humble",
                    AuthorId = "1",
                    ImageFileName = "test image",
                },
                new Post()
                {
                    Id = Guid.NewGuid(),
                    Title = "#coolpic",
                    Description = "The most picture ever",
                    AuthorId = "2",
                    ImageFileName = "test image",
                }
            );
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> Data already seeded.");
        }
    }
}