using ContactManager.Application.Interfaces;
using ContactManager.Application.Models;
using ContactManager.Persistence.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dropbox.Api;
using Newtonsoft.Json;

namespace ContactManager.Application.Services
{
    public class EmailService : IEmailService
    {
        private DropboxClient? _dropboxClient;
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _refreshToken;
        private static DateTime _tokenExpirationTime;
        private readonly ILogger<EmailService> _logger;
        private const string _dropboxApiUrl = "https://api.dropboxapi.com/oauth2/token";
        private string? _accessToken;

        public EmailService(DropboxCredentials dropboxCredentials, ILogger<EmailService> logger)
        {
            _clientId = dropboxCredentials.ClientId;
            _clientSecret = dropboxCredentials.ClientSecret;
            _refreshToken = dropboxCredentials.RefreshToken;
            _logger = logger;

            CheckAccessToken().GetAwaiter();
        }

        public async Task SendEmailByGmailAsync(ApplicationUser user, EmailTemplates emailTemplate,
            EmailSettings emailSettings, Dictionary<string, string>? emailBodyValues = null)
        {
            try
            {
                using (var message = new MailMessage())
                using (var smtp = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort))
                {
                    message.To.Add(new MailAddress(user.Email, user.UserName));
                    message.From = new MailAddress(emailSettings.Email, emailSettings.Name);
                    message.Subject = GetEmailSubject(emailTemplate);
                    message.Body = await BuildEmailBodyAsync(emailTemplate, emailBodyValues);
                    message.IsBodyHtml = true;

                    smtp.Credentials = new NetworkCredential(emailSettings.SmtpUser, emailSettings.SmtpPassword);
                    smtp.EnableSsl = emailSettings.SmtpSSL;

                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending an email.\nException: {Exception}", ex.ToString());
            }
        }

        private string GetEmailSubject(EmailTemplates emailTemplate)
        {
            string templateName = emailTemplate.ToString();
            return Regex.Replace(templateName, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
        }

        private async Task<string> BuildEmailBodyAsync(EmailTemplates emailTemplate,
            Dictionary<string, string> emailBodyValues)
        {
            string emailBody = await GetEmailTemplateAsync(emailTemplate);

            if (emailBodyValues != null)
            {
                foreach (var emailBodyValue in emailBodyValues)
                {
                    emailBody = emailBody.Replace($"[{emailBodyValue.Key}]", emailBodyValue.Value);
                }
            }

            return emailBody;
        }

        private async Task<string> GetEmailTemplateAsync(EmailTemplates emailTemplate)
        {
            await CheckAccessToken();

            try
            {
                var response = await _dropboxClient.Files.DownloadAsync($"/EmailTemplates/{emailTemplate}.txt");

                var fileContent = await response.GetContentAsStringAsync();

                return fileContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the email template.\nException: {Exception}", ex.ToString());

                return string.Empty;
            }
        }

        private async Task<string> RefreshAccessToken()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set Basic Authentication headers
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));

                    // Prepare request content
                    var content = new StringContent($"grant_type=refresh_token&refresh_token={_refreshToken}",
                        Encoding.UTF8, "application/x-www-form-urlencoded");

                    // Send POST request to Dropbox API to refresh access token
                    var response = await client.PostAsync(_dropboxApiUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    var tokenRefreshResponse = JsonConvert.DeserializeObject<DropboxRefreshTokenResponse>(responseString);

                    // Update access token and expiration time
                    _accessToken = tokenRefreshResponse.Access_Token;
                    _tokenExpirationTime = DateTime.Now.AddSeconds(tokenRefreshResponse.Expires_In);

                    return tokenRefreshResponse.Access_Token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing the access token.\nException: {Exception}", ex.ToString());

                return string.Empty;
            }
        }

        private async Task CheckAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.Now >= _tokenExpirationTime)
            {
                await RefreshAccessToken();
                _dropboxClient = new DropboxClient(_accessToken);
            }
        }
    }
}
