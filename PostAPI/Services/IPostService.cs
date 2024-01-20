using System.Collections.Generic;
using System.Threading.Tasks;
using PostAPI.Models;
using PostAPI.Models.DTOs;

namespace PostAPI.Services;

public interface IPostService
{
    Task<Post> GetPostByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetAllPostsInUserProfileAsync(string authorId);
    Task<IEnumerable<Post>> GetAllPosts();
    Task<Post> CreatePostAsync(Post Post);
    Task<bool> PutPostAsync(Post Post);
    Task<bool> DeletePostAsync(Guid id);
}