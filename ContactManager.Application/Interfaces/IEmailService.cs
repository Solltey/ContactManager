using ContactManager.Application.Models;
using ContactManager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailByGmailAsync(ApplicationUser user, EmailTemplates emailTemplate,
            EmailSettings emailSettings, Dictionary<string, string>? emailBodyValues = null);
    }
}
