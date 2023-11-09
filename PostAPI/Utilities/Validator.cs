using PostAPI.Models.DTOs;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PostAPI.Utilities
{
    public class Validator
    {
        public static bool CheckInputInvalid(PostDTO postDTO) => postDTO == null || string.IsNullOrWhiteSpace(postDTO.Title) ||
                string.IsNullOrWhiteSpace(postDTO.Description) ||
                postDTO.AuthorId == null;
    }
}
