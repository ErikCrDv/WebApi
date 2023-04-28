using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Validations;

namespace WebApi.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is requireed!")]
        [StringLength(maximumLength:120, ErrorMessage = "Field {0} only max {1} characters.")]
        [FirstLetterCapitalized]
        public string Name { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }


    }
}
