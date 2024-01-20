﻿using Microsoft.EntityFrameworkCore;
using PostAPI.Models;

namespace PostAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public async Task CleanInvalidPostsAsync()
        {
            var invalidPosts = Posts.Where(p => p.AuthorId == null);
            if (await invalidPosts.AnyAsync())
            {
                Posts.RemoveRange(invalidPosts);
                await SaveChangesAsync();
            }
        }
    }
}
