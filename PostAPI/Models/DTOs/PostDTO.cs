﻿namespace PostAPI.Models.DTOs
{
    public class PostDTO
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long AuthorId { get; set; }

    }
}
