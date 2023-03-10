using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task<UserViewModel> GetUserByUserNameAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return new UserViewModel
            {
                Email = currentUser!.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, password);

            return checkOldPassword;
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }
            await _userManager.UpdateSecurityStampAsync(currentUser); //açık tarayıcı varsa şifre değiştirdiğimiz için stamp güncellemesi yapılmalı o yüzden oralarda çıkış yapmamlı

            await _signInManager.SignOutAsync(); //cookienin güncellenmesi için çıkış yapıp tekrar giriş yapıyoruz
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);

            return (true, null);
        }

        public async Task<UserEditViewModel> GetUserEditViewAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            return new UserEditViewModel
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                PhoneNumber = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };
        }

        public SelectList GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel userEditViewModel, string userName)
        {

            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            currentUser.UserName = userEditViewModel.UserName;
            currentUser.Email = userEditViewModel.Email;
            currentUser.BirthDate = userEditViewModel.BirthDate;
            currentUser.City = userEditViewModel.City;
            currentUser.Gender = userEditViewModel.Gender;
            currentUser.PhoneNumber = userEditViewModel.PhoneNumber;

            if (userEditViewModel.Picture != null && userEditViewModel.Picture.Length > 0)
            {
                var wwwRootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid()}{Path.GetExtension(userEditViewModel.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwRootFolder.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await userEditViewModel.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                return (false, updateToUserResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);

            await _signInManager.SignOutAsync();

            //kullanıcı güncellendikten sonra çıkıp girmesi için yazılan blok. çünkü claim güncellenmeli. eğer doğum tarihi var ise birthdate claim i ile birlikte giriş yapmasını
            //yok ise claimsiz giriş yapmasını sağlıyoruz.
            if (userEditViewModel.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("BirthDate", currentUser.BirthDate!.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }

            return (true, null);
        }

        public List<ClaimViewModel> GetClaims(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.Select(u => new ClaimViewModel()
            {
                Issuer = u.Issuer,
                Type = u.Type,
                Value = u.Value
            }).ToList();
        }
    }
}
