using ContactManager.Application.Interfaces;
using ContactManager.Persistence.Entities;
using ContactManager.WebUI.Interfaces;
using ContactManager.WebUI.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using static Dropbox.Api.Sharing.ListFileMembersIndividualResult;

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

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequest);    
            }

            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                ModelState.AddModelError("", "Incorrect Email or Password");
                return View(loginRequest);
            }

            var canSignIn = await _signInManager.CanSignInAsync(user);

            if (!canSignIn)
            {
                ModelState.AddModelError("", "Your Account may be Locked Out");
                return View(loginRequest);
            }

            var tokenRequest = _registrationManager.GetTokenRequest(user);

            var jwt = await _jwtService.GetTokenAsync(tokenRequest);

            HttpContext.Response.Cookies.Append("access_token", jwt,
            new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(240)
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            HttpContext.Response.Cookies.Delete("access_token");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registrationRequest);
            }

            var newUser = await _registrationManager.RegisterUserAsync(
                registrationRequest.Username,
                registrationRequest.Email,
                registrationRequest.Password);

            if (newUser == null)
            {
                return View(registrationRequest);
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            var confirmationLink = Url.Link("default", new { controller = "Account", action = "ConfirmEmail", userId = newUser.Id, code });

            await _registrationManager.SendConfirmationEmailAsync(newUser, confirmationLink);

            return RedirectToAction("Index", "Home");
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
