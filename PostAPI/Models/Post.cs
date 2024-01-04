using System.ComponentModel.DataAnnotations;
namespace PostAPI.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public string? ImageFileName { get; set; }
        [Required]
        public string? FileExtension { get; set; }
    }
}
