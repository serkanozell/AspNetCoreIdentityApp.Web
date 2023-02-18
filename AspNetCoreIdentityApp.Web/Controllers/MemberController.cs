﻿using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserViewModel
            {
                Email = currentUser!.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber
            };

            return View(userViewModel);
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
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

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, passwordChangeViewModel.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Old Password incorrect");

                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(e => e.Description).ToList());

                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser); //açık tarayıcı varsa şifre değiştirdiğimiz için stamp güncellemesi yapılmalı o yüzden oralarda çıkış yapmamlı

            await _signInManager.SignOutAsync(); //cookienin güncellenmesi için çıkış yapıp tekrar giriş yapıyoruz
            await _signInManager.PasswordSignInAsync(currentUser, passwordChangeViewModel.PasswordNew, true, false);

            TempData["SuccessMessage"] = "You password changed successfully.";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.GenderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userEditViewModel = new UserEditViewModel
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                PhoneNumber = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City
            };

            return View(userEditViewModel);
        }
    }
}
