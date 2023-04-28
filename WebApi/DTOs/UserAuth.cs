using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class UserAuth
    {
        [Required]
        [EmailAddress]

        public string Email { get; set; }

        [Required]
        public string  Password { get; set; }
    }
}
