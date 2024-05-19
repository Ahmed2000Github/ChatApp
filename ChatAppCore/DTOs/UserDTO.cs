
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatAppCore.DTOs
{
    public class UserDTO
    {
        [Required]
        [StringLength(20, ErrorMessage = "First name must be at least 4 characters long.", MinimumLength = 4)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Last name must be at least 4 characters long.", MinimumLength = 4)]
        public string LastName { get; set; }
        public IFormFile? AvatarImage { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
