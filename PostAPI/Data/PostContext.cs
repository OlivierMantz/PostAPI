using PostAPI.Models;
using Microsoft.EntityFrameworkCore;
using PostAPI.Models;

namespace PostAPI.Data
{
    public class PostContext : DbContext
    {
        public PostContext(DbContextOptions<PostContext> options) : base(options)
        {
        }

        public DbSet<Post> Post { get; set; } = null!;
    }
}
