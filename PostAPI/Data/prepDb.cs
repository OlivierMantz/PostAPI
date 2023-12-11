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
                    Id = 1,
                    Title = "Some picture",
                    Description = "#hashtag",
                    AuthorId = "1",
                    ImageUrl = "test image",
                },
                new Post()
                {
                    Id = 2,
                    Title = "Second Ansel Adams ",
                    Description = "#humble",
                    AuthorId = "1",
                    ImageUrl = "test image",
                },
                new Post()
                {
                    Id = 3,
                    Title = "#coolpic",
                    Description = "The most picture ever",
                    AuthorId = "2",
                    ImageUrl = "test image",
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