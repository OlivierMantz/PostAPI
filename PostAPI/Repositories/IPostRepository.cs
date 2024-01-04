using System.Collections.Generic;
using System.Threading.Tasks;
using PostAPI.Models;

namespace PostAPI.Repositories
{
    public interface IPostRepository
    {
        Task<Post> GetPostByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetAllPostsInUserProfileAsync(string authorId);
        Task<IEnumerable<Post>> GetAllPosts();
        Task<Post> CreatePostAsync(Post Post);
        Task<bool> PutPostAsync(Post Post);
        Task<bool> DeletePostAsync(Guid id);
        // Helper method
        Task<bool> PostExistsAsync(Guid id);
    }

}
