using System.Collections.Generic;
using System.Threading.Tasks;
using PostAPI.Models;

namespace PostAPI.Services;

public interface IPostService
{
    Task<List<Post>> GetPostsAsync();
    Task<Post> GetPostByIdAsync(long id);
    Task CreatePostAsync(Post Post);
    Task<bool> PutPostAsync(Post Post);
    Task<bool> DeletePostAsync(long id);
}