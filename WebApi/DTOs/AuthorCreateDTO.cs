using System.ComponentModel.DataAnnotations;
using WebApi.Validations;

namespace WebApi.DTOs
{
    public class AuthorCreateDTO
    {
        [Required(ErrorMessage = "Field {0} is requireed!")]
        [StringLength(maximumLength: 120, ErrorMessage = "Field {0} only max {1} characters.")]
        [FirstLetterCapitalized]
        public string Name { get; set; }
    }
}
