using ContactManager.Application.Interfaces;
using ContactManager.Application.Models;
using ContactManager.Persistence.Entities;
using ContactManager.WebUI.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.WebUI.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountManager(IConfiguration configuration,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _emailService = emailService;
            _userManager = userManager;
        }

        public TokenRequest GetTokenRequest(ApplicationUser user)
        {
            var tokenRequest = new TokenRequest
            {
                User = user,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                ExpirationTime = _configuration["JwtSettings:ExpirationTimeInMinutes"],
                SecurityKey = _configuration["JwtSettings:SecurityKey"]
            };

            return tokenRequest;
        }

        public async Task<ApplicationUser> RegisterUserAsync(string username, string email, string password = "Default123")
        {
            var newUser = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = username,
                Email = email,
                LockoutEnabled = false
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
                return null;

            return newUser;
        }

        public async Task SendConfirmationEmailAsync(ApplicationUser user, string confirmationLink)
        {
            Dictionary<string, string> emailBodyValues = new Dictionary<string, string>
            {
                { "dateTime", DateTime.Now.ToString("dd/MM/yyyy") },
                { "username", user.UserName },
                { "email", user.Email },
                { "confirmationLink", confirmationLink }
            };

            var emailSettings = GetDefaultEmailSettings();

            await _emailService.SendEmailByGmailAsync(user, EmailTemplates.ConfirmationEmail, emailSettings, emailBodyValues);
        }

        private EmailSettings GetDefaultEmailSettings()
        {
            var emailSettings = new EmailSettings
            {
                Email = _configuration["DefaultEmailSettings:Email"],
                Name = _configuration["DefaultEmailSettings:Name"],
                SmtpUser = _configuration["DefaultEmailSettings:SmtpUser"],
                SmtpPassword = _configuration["DefaultEmailSettings:SmtpPassword"],
                SmtpHost = _configuration["DefaultEmailSettings:SmtpHost"],
                SmtpPort = Int32.Parse(_configuration["DefaultEmailSettings:SmtpPort"]),
                SmtpSSL = Boolean.Parse(_configuration["DefaultEmailSettings:SmtpSSL"])
            };

            return emailSettings;
        }
    }
}
