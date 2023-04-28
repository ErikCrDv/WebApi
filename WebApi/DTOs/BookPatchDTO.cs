using System.ComponentModel.DataAnnotations;
using WebApi.Validations;

namespace WebApi.DTOs
{
    public class BookPatchDTO
    {
        [FirstLetterCapitalized]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Title { get; set; }
        public DateTime ReleseDate { get; set; }

        public List<int> AuthorsId { get; set; }
    }
}
