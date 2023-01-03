using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Validations;

namespace WebApi.Models
{
    public class Author : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Field {0} is requireed!")]
        [StringLength(maximumLength:120, ErrorMessage = "Field {0} only max {1} characters.")]
        //[FirstLetterCapitalized]
        public string Name { get; set; }

        //[Range(18, 120)]
        //[NotMapped]
        //public int Age { get; set; }
        //[CreditCard]
        //[NotMapped]
        //public string CreditCard { get; set; }
        //[Url]
        //[NotMapped]
        //public string UrlBlog { get; set; }
        public List<Book> Books { get; set; }


        //[NotMapped]
        //public int Minor { get; set; }
        //[NotMapped]
        //public int Major { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if( !string.IsNullOrEmpty(Name))
            {
                var firstLetter = Name[0].ToString();
                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("Fisrt Letter mnust be upercase",
                        new string[] { nameof(Name)});
                }
            }

            //if( Minor > Major)
            //{
            //    yield return new ValidationResult("Minor FIeld should not be greater that Major",
            //        new string[] { nameof(Minor) });
            //}
        }
    }
}
