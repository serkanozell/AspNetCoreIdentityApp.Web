using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Role-Action")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(r => new RoleViewModel()
            {
                RoleId = r.Id!,
                RoleName = r.Name!
            }).ToListAsync();

            return View(roles);
        }

        [Authorize(Roles = "Role-Action")]
        public IActionResult RoleAdd()
        {
            return View();
        }

        [Authorize(Roles = "Role-Action")]
        [HttpPost]
        public async Task<IActionResult> RoleAdd(RoleAddViewModel roleAddViewModel)
        {
            var result = await _roleManager.CreateAsync(new AppRole()
            {
                Name = roleAddViewModel.RoleName
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);

                return View(result);
            }

            TempData["SuccessMessage"] = "Role added successfuly";

            return RedirectToAction(nameof(RoleController.Index));
        }

        [Authorize(Roles = "Role-Action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate == null)
                throw new Exception("There is no role to update");

            var roleUpdateViewModel = new RoleUpdateViewModel()
            {
                RoleId = roleToUpdate.Id!,
                RoleName = roleToUpdate.Name!
            };

            return View(roleUpdateViewModel);
        }

        [Authorize(Roles = "Role-Action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel roleUpdateViewModel)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(roleUpdateViewModel.RoleId!);

            if (roleToUpdate == null)
                throw new Exception("There is no role to update");

            roleToUpdate.Name = roleUpdateViewModel.RoleName;

            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Role updated successfully";

            return View(roleUpdateViewModel);
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
                throw new Exception("There is no role to delete");

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
                throw new Exception(result.Errors.Select(e => e.Description).First());

            TempData["SuccessMessage"] = "Role deleted successfuly";

            return RedirectToAction(nameof(RoleController.Index));
        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = (await _userManager.FindByIdAsync(id))!;

            ViewBag.userId = id;

            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var assignRoleToUserViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name!
                };

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                assignRoleToUserViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(assignRoleToUserViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> userRoles)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;

            foreach (var role in userRoles)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.RoleName);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.RoleName);
                }
            }

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            if (userToAssignRoles.UserName == currentUser.UserName)
            {
                await _signInManager.SignOutAsync();

                await _signInManager.SignInAsync(currentUser, true);
            }

            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}
