using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Models
{
    public class EmailSettings
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? SmtpUser { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSSL { get; set; }
    }
}
