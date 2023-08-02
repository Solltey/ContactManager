using ContactManager.Persistence.Context;
using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebUI.Attributes
{
    public class UniqueUsernameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            var username = value as string;

            if (string.IsNullOrEmpty(username))
                return ValidationResult.Success;

            bool isEmailUnique = !dbContext.Users.Any(u => u.UserName == username);

            if (!isEmailUnique)
                return new ValidationResult("This username is already in use.");

            return ValidationResult.Success;
        }
    }
}
