using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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

            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "You SignUp request success.";
                return RedirectToAction("SignUp");
            }

            ModelState.AddModelErrorList(identityResult.Errors.Select(e => e.Description).ToList());

            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var user = await _userManager.FindByEmailAsync(signInViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password incorrect");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, signInViewModel.Password, signInViewModel.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Your account locked for 3 minutes" });

                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { "Email or password incorrect.", $" Failure login attemt count is : {await _userManager.GetAccessFailedCountAsync(user)}" });

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}