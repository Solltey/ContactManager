using ContactManager.WebUI.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebUI.Models.Authentication
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [UniqueUsername(ErrorMessage = "Username is already taken")]
        [MinLength(5, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(25, ErrorMessage = "Password must not exceed 20 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [UniqueEmail(ErrorMessage = "Email is already taken")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(20, ErrorMessage = "Password must not exceed 20 characters")]
        public string? Password { get; set; }
    }
}
