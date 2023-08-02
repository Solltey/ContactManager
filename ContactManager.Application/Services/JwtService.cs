using ContactManager.Application.Interfaces;
using ContactManager.Application.Models;
using ContactManager.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<JwtService> _logger;

        public JwtService(UserManager<ApplicationUser> userManager,
            ILogger<JwtService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync(TokenRequest tokenRequest)
        {
            try
            {
                if (tokenRequest.User == null)
                {
                    throw new ArgumentNullException(nameof(tokenRequest.User), "User can't be null.");
                }

                if (tokenRequest.SecurityKey == null)
                {
                    throw new ArgumentNullException(nameof(tokenRequest.SecurityKey), "SecurityKey can't be null.");
                }

                var jwtOptions = new JwtSecurityToken(
                        issuer: tokenRequest.Issuer,
                        audience: tokenRequest.Audience,
                        claims: await GetClaimsAsync(tokenRequest.User),
                        expires: DateTime.Now.AddMinutes(
                            Convert.ToDouble(tokenRequest.ExpirationTime)),
                        signingCredentials: GetSigningCredentials(tokenRequest.SecurityKey)
                    );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtOptions);

                return accessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating Jwt token.\nException: {Exception}", ex.ToString());

                return null;
            }
        }

        private SigningCredentials GetSigningCredentials(string securityKey)
        {
            var key = Encoding.UTF8.GetBytes(securityKey);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret,
                SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
