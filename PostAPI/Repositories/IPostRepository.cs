using System.Collections.Generic;
using System.Threading.Tasks;
using PostAPI.Models;

namespace PostAPI.Repositories
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(long id);
        Task CreatePostAsync(Post Post);
        Task<bool> PutPostAsync(Post Post);
        Task<bool> DeletePostAsync(long id);
        // Helper method
        Task<bool> PostExistsAsync(long id);
    }

}
