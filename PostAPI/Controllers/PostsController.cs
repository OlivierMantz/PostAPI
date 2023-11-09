using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostAPI.Models;
using PostAPI.Models.DTOs;
using PostAPI.Services;

using PostAPI.Utilities;

namespace PostAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        private static PostDTO PostToDto(Post post)
        {
            var postDto = new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                AuthorId = post.AuthorId
            };
            return postDto;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<List<PostDTO>>> GetPosts()
        {
            var posts = await _postService.GetPostsAsync();
            var postDtos = posts.Select(PostToDto).ToList();
            return Ok(postDtos);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDTO>> GetPost(long id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid id parameter. The id must be a positive number.");
            }

            var user = await _postService.GetPostByIdAsync(id);

            if (user == null)
            {
                return NotFound("Post not found.");
            }

            var userDto = PostToDto(user);
            return Ok(userDto);
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostDTO>> PostPost(PostDTO postDto)
        {
            if (Validator.CheckInputInvalid(postDto))
            {
                return Problem("One or more invalid inputs");
            }

            var post = new Post
            {
                Title = postDto.Title,
                Description = postDto.Description,
                AuthorId = postDto.AuthorId
            };

            await _postService.CreatePostAsync(post);

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, PostToDto(post));
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(long id, PostDTO postDto)
        {
            if (id != postDto.Id)
            {
                return BadRequest("The provided id parameter does not match the user's id.");
            }

            var existingPost = await _postService.GetPostByIdAsync(id);

            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Title = postDto.Title;
            existingPost.Description = postDto.Description;
            existingPost.AuthorId = postDto.AuthorId;

            await _postService.PutPostAsync(existingPost);

            return NoContent();
        }


        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var existingUser = await _postService.DeletePostAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _postService.DeletePostAsync(id);

            return NoContent();
        }
    }
}
