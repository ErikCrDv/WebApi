using System.ComponentModel.DataAnnotations;
using WebApi.Validations;

namespace WebApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [FirstLetterCapitalized]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime? ReleseDate { get; set; }

        public List<Comment> Comments { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }

    }
}
