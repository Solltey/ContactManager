using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Models
{
    public class DropboxRefreshTokenResponse
    {
        public string? Access_Token { get; set; }
        public string? Token_Type { get; set; }
        public double Expires_In { get; set; }
    }
}
