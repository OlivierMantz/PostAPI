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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PostAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        private string GetCurrentUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

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
                AuthorId = post.AuthorId,
                ImageFileName = post.ImageFileName,
                FileExtension = post.FileExtension,
            };
            return postDto;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDTO>> GetPostByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid id parameter. The id must be a positive number.");
            }

            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var postDto = PostToDto(post);
            return Ok(postDto);
        }

        // GET: api/Posts/Users/1
        [HttpGet("users/{authorId}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetAllPostsInUserProfileAsync(string authorId)
        {
            var posts = await _postService.GetAllPostsInUserProfileAsync(authorId);
            var postDtos = posts.Select(PostToDto).ToList();
            return Ok(postDtos);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            try
            {
                var posts = await _postService.GetAllPosts();
                var postDTOs = posts.Select(post => PostToDto(post)).ToList();

                return Ok(postDTOs);
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDTO createPostDTO)
        {
            if (Validator.CheckInputInvalid(createPostDTO))
            {
                return Problem("One or more invalid inputs");
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var post = new Post
            {
                Title = createPostDTO.Title,
                Description = createPostDTO.Description,
                AuthorId = userId,
                ImageFileName = createPostDTO.ImageFileName,
                FileExtension = createPostDTO.FileExtension,
            };

            var createdPost = await _postService.CreatePostAsync(post);
            if (createdPost == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var createdPostDTO = PostToDto(createdPost);
            return Ok(createdPostDTO);
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(Guid id, PostDTO postDto)
        {
            if (id != postDto.Id)
            {
                return BadRequest("The provided id parameter does not match the post's id.");
            }

            var existingPost = await _postService.GetPostByIdAsync(id);

            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Title = postDto.Title;
            existingPost.Description = postDto.Description;
            existingPost.AuthorId = postDto.AuthorId;
            existingPost.ImageFileName = postDto.ImageFileName;


            await _postService.PutPostAsync(existingPost);

            return NoContent();
        }


        // DELETE: api/Posts/5
        [Authorize(Roles = "User, Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();

            if (existingPost.AuthorId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _postService.DeletePostAsync(id);

            return NoContent();
        }
    }
}
