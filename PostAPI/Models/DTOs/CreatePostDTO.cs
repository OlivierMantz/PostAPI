namespace PostAPI.Models.DTOs
{
    public class CreatePostDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string ImageFileName { get; set; }
        public string FileExtension { get; set; }

    }
}
