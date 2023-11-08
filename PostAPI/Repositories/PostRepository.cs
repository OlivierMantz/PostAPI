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
        private readonly PostContext _PostContext;

        public PostRepository(PostContext PostContext)
        {
            _PostContext = PostContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _PostContext.Post.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(long id)
        {
            return await _PostContext.Post.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task CreatePostAsync(Post Post)
        {
            if (Post == null)
            {
                throw new ArgumentNullException(nameof(Post));
            }
            await _PostContext.Post.AddAsync(Post);
            await _PostContext.SaveChangesAsync();
        }

        public async Task<bool> PutPostAsync(Post Post)
        {
            _PostContext.Post.Update(Post);
            var updated = await _PostContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(long id)
        {
            var Post = await _PostContext.Post.FindAsync(id);
            if (Post != null)
            {
                _PostContext.Post.Remove(Post);
                var deleted = await _PostContext.SaveChangesAsync();
                return deleted > 0;
            }

            return false;
        }

        public async Task<bool> PostExistsAsync(long PostId)
        {
            return await _PostContext.Post.AnyAsync(u => u.Id == PostId);
        }
    }
}