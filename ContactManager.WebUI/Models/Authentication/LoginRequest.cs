using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebUI.Models.Authentication
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(20, ErrorMessage = "Password must not exceed 20 characters")]
        public string? Password { get; set; }
    }
}
