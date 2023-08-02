using ContactManager.Application.Models;
using ContactManager.Persistence.Entities;

namespace ContactManager.WebUI.Interfaces
{
    public interface IAccountManager
    {
        Task SendConfirmationEmailAsync(ApplicationUser user, string confirmationLink);
        Task<ApplicationUser> RegisterUserAsync(string username, string email, string password);
        TokenRequest GetTokenRequest(ApplicationUser user);
    }
}
