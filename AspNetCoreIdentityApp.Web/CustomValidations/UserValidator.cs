using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var erros = new List<IdentityError>();

            var isDigit = int.TryParse(user.UserName![0].ToString(), out _);

            if (isDigit)
            {
                erros.Add(new IdentityError()
                {
                    Code = "UserNameContainFirstLetterDigit",
                    Description = "Username cannot starts with digit"
                });
            }

            if (erros.Any())
            {
                return Task.FromResult(IdentityResult.Failed(erros.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
