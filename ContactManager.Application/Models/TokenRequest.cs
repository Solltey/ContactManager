using ContactManager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Models
{
    public class TokenRequest
    {
        public ApplicationUser? User { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? ExpirationTime { get; set; }
        public string? SecurityKey { get; set; }
    }
}
