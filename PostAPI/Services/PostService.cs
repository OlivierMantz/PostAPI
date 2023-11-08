﻿using System.Collections.Generic;
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


        public async Task<List<Post>> GetPostsAsync()
        {
            return await _PostRepository.GetPostsAsync();
        }

        public async Task<Post> GetPostByIdAsync(long id)
        {
            return await _PostRepository.GetPostByIdAsync(id);
        }

        public async Task CreatePostAsync(Post Post)
        {
            await _PostRepository.CreatePostAsync(Post);
        }

        public async Task<bool> PutPostAsync(Post Post)
        {
            return await _PostRepository.PutPostAsync(Post);
        }

        public async Task<bool> DeletePostAsync(long id)
        {
            return await _PostRepository.DeletePostAsync(id);
        }

        public async Task<bool> PostExistsAsync(long id)
        {
            return await _PostRepository.PostExistsAsync(id);
        }
    }
}