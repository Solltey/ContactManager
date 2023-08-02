using ContactManager.Application.Interfaces;
using ContactManager.Persistence.Entities;
using ContactManager.WebUI.Interfaces;
using ContactManager.WebUI.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountManager _registrationManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            SignInManager<ApplicationUser> signInManager,
            IAccountManager registrationManager)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _signInManager = signInManager;
            _registrationManager = registrationManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new LoginResult
                {
                    Success = false,
                    DataValidationErrors = errors.ToList()
                });
            }

            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Unauthorized(new LoginResult
                {
                    Success = false
                });
            }

            var canSignIn = await _signInManager.CanSignInAsync(user);

            if (!canSignIn)
            {
                return Unauthorized(new LoginResult
                {
                    Success = false
                });
            }

            var tokenRequest = _registrationManager.GetTokenRequest(user);

            var jwt = await _jwtService.GetTokenAsync(tokenRequest);

            return Ok(new LoginResult
            {
                Success = true,
                Token = jwt
            });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] RegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new RegistrationResult
                {
                    Success = false,
                    DataValidationErrors = errors.ToList()
                });
            }

            var newUser = await _registrationManager.RegisterUserAsync(
                registrationRequest.Username,
                registrationRequest.Email,
                registrationRequest.Password);

            if (newUser == null)
            {
                return BadRequest(new RegistrationResult
                {
                    Success = false
                });
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            var confirmationLink = Url.Link("default", new { controller = "Account", action = "ConfirmEmail", userId = newUser.Id, code });

            await _registrationManager.SendConfirmationEmailAsync(newUser, confirmationLink);

            return Ok(new RegistrationResult
            {
                Success = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            return View("EmailConfirmed");
        }
    }
}
