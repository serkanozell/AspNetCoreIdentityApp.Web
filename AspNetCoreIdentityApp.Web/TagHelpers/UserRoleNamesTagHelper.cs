using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserRoleNamesTagHelper : TagHelper
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public string UserId { get; set; } = null!;


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            var userRoles = await _userManager.GetRolesAsync(user!);

            var stringBuilder = new StringBuilder();

            userRoles.ToList().ForEach(u =>
            {
                stringBuilder.Append(@$"<span class='badge bg-secondary mx-1'>{u.ToLower()}</span>");
            });

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
