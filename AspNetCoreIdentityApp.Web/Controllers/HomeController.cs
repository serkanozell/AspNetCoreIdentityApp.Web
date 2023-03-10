using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new AppUser()
            {
                UserName = signUpViewModel.UserName,
                PhoneNumber = signUpViewModel.PhoneNumber,
                Email = signUpViewModel.Email
            }, signUpViewModel.PasswordConfirm);

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(e => e.Description).ToList());

                return View();
            }

            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());

            var currentUser = await _userManager.FindByNameAsync(signUpViewModel.UserName);

            var claimResult = await _userManager.AddClaimAsync(currentUser!, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(e => e.Description).ToList());

                return View();
            }

            TempData["SuccessMessage"] = "You SignUp request success.";

            return RedirectToAction("SignUp");

        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl ??= Url.Action("Index", "Home");

            var user = await _userManager.FindByEmailAsync(signInViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password incorrect");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, signInViewModel.Password, signInViewModel.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Your account locked for 3 minutes" });

                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { "Email or password incorrect.", $" Failure login attemt count is : {await _userManager.GetAccessFailedCountAsync(user)}" });

                return View();
            }

            if (user.BirthDate.HasValue)
                await _signInManager.SignInWithClaimsAsync(user, signInViewModel.RememberMe, new[] { new Claim("BirthDate", user.BirthDate.Value.ToString()) });

            return Redirect(returnUrl!);
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var hasUser = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "The username with this e-mail could not be found");

                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new
            {
                userId = hasUser.Id,
                Token = passwordResetToken
            }, HttpContext.Request.Scheme);

            //https://localhost:7026?userId=12321&token=3r32rdfqweşjpjf03ı2jf
            //alkcehqhzcjvaldc

            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Password reset e-mail sended to your e-mail address";

            return RedirectToAction("ForgotPassword");
        }


        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["user_id"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var userId = TempData["user_id"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Error found");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "User can't found");

                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, resetPasswordViewModel.Password!);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password changed successfully";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(e => e.Description).ToList());

                return View();
            }

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}