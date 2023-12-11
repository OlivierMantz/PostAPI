using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostAPI.Data;
using PostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PostAPI.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _PostContext;

        public PostRepository(ApplicationDbContext PostContext)
        {
            _PostContext = PostContext;
        }

        public async Task<Post> GetPostByIdAsync(long id)
        {
            return await _PostContext.Posts.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Post>> GetAllPostsInUserProfileAsync(string authorId)
        {
            return await _PostContext.Posts
                                 .Where(c => c.AuthorId == authorId)
                                 .ToListAsync();
        }

        public async Task<Post> CreatePostAsync(Post Post)
        {
            if (Post == null)
            {
                throw new ArgumentNullException(nameof(Post));
            }
            await _PostContext.Posts.AddAsync(Post);
            await _PostContext.SaveChangesAsync();

            return Post;
        }

        public async Task<bool> PutPostAsync(Post Post)
        {
            _PostContext.Posts.Update(Post);
            var updated = await _PostContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(long id)
        {
            var Post = await _PostContext.Posts.FindAsync(id);
            if (Post != null)
            {
                _PostContext.Posts.Remove(Post);
                var deleted = await _PostContext.SaveChangesAsync();
                return deleted > 0;
            }

            return false;
        }

        public async Task<bool> PostExistsAsync(long PostId)
        {
            return await _PostContext.Posts.AnyAsync(u => u.Id == PostId);
        }
    }
}