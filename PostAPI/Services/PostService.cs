using System.Collections.Generic;
using System.Threading.Tasks;
using PostAPI.Models;
using PostAPI.Repositories;

namespace PostAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _PostRepository;

        public PostService(IPostRepository PostRepository)
        {
            _PostRepository = PostRepository;
        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            return await _PostRepository.GetPostByIdAsync(id);
        }

        public async Task<IEnumerable<Post>> GetAllPostsInUserProfileAsync(string authorId)
        {
            return await _PostRepository.GetAllPostsInUserProfileAsync(authorId);
        }
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            return await _PostRepository.GetAllPosts();
        }

        public async Task<Post> CreatePostAsync(Post Post)
        {
            await _PostRepository.CreatePostAsync(Post);
            return Post;
        }

        public async Task<bool> PutPostAsync(Post Post)
        {
            return await _PostRepository.PutPostAsync(Post);
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            return await _PostRepository.DeletePostAsync(id);
        }

        public async Task<bool> PostExistsAsync(Guid id)
        {
            return await _PostRepository.PostExistsAsync(id);
        }
    }
}