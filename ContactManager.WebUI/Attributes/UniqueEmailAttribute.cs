using ContactManager.Persistence.Context;
using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebUI.Attributes
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            var email = value as string;

            if (string.IsNullOrEmpty(email))
                return ValidationResult.Success;

            bool isEmailUnique = !dbContext.Users.Any(u => u.Email == email);

            if (!isEmailUnique)
                return new ValidationResult("This email is already in use.");

            return ValidationResult.Success;
        }
    }
}
