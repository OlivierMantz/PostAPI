namespace PostAPI.Models.DTOs
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string AuthorId { get; set; }
        public string? ImageFileName { get; set; }
        public string? FileExtension { get; set; }

    }
}
