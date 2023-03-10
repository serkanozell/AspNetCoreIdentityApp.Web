using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity!.Name!;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserByUserNameAsync(userName));
        }

        public async Task LogOut()
        {
            await _memberService.LogOutAsync();
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _memberService.CheckPasswordAsync(userName, passwordChangeViewModel.PasswordOld))
            {
                ModelState.AddModelError(string.Empty, "Old Password incorrect");

                return View();
            }

            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);

                return View();
            }

            TempData["SuccessMessage"] = "You password changed successfully.";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.GenderList = _memberService.GetGenderSelectList();

            return View(await _memberService.GetUserEditViewAsync(userName));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (isSuccess, errors) = await _memberService.EditUserAsync(userEditViewModel, userName);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);

                return View();
            }

            TempData["SuccessMessage"] = "User details changed successfully";

            return View(await _memberService.GetUserEditViewAsync(userName));
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View(_memberService.GetClaims(User));
        }

        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult ViolencePage()
        {
            return View();
        }

        public async Task<IActionResult> AccessDenied(string returnUrl)
        {
            string message = string.Empty;

            message = "You don't have permission to view this page. Please contact to website admin for permissions";

            ViewBag.Message = message;

            return View();
        }
    }
}
