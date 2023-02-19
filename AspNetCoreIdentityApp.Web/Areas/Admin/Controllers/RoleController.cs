using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(r => new RoleViewModel()
            {
                RoleId = r.Id!,
                RoleName = r.Name!
            }).ToListAsync();

            return View(roles);
        }

        public IActionResult RoleAdd()
        {
            return View();
        }

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
    }
}
