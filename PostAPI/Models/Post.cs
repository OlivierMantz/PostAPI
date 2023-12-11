using System.ComponentModel.DataAnnotations;
namespace PostAPI.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public string? ImageUrl { get; set; }

    }
}
