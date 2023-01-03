using System.ComponentModel.DataAnnotations;

namespace WebApi.Validations
{
    public class FirstLetterCapitalized : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if( value == null || string.IsNullOrEmpty( value.ToString() ) )
            {
                return ValidationResult.Success;
            }

            var fisrtLetter = value.ToString()[0].ToString();
            if (fisrtLetter != fisrtLetter.ToUpper() )
            {
                return new ValidationResult("First letter must be uppercase");
            }

            return ValidationResult.Success;    
        }
    }
}
