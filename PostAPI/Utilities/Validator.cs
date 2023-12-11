using PostAPI.Models.DTOs;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PostAPI.Utilities
{
    public class Validator
    {
        public static bool CheckInputInvalid(CreatePostDTO createPostDTO) => createPostDTO == null || string.IsNullOrWhiteSpace(createPostDTO.Title) ||
                string.IsNullOrWhiteSpace(createPostDTO.Description);
    }
}
