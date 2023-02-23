using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.PermissionsRoot;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Seeds
{
    public class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");

            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");

            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

                await AddReadPermission(roleManager, basicRole);
            }

            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" });

                var basicRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;

                await AddReadPermission(roleManager, basicRole);
                await AddCreateAndUpdatePermission(roleManager, basicRole);
            }

            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });

                var basicRole = (await roleManager.FindByNameAsync("AdminRole"))!;

                await AddReadPermission(roleManager, basicRole);
                await AddCreateAndUpdatePermission(roleManager, basicRole);
                await AddDeletePermission(roleManager, basicRole);
            }

        }

        public static async Task AddReadPermission(RoleManager<AppRole> roleManager, AppRole role)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Read));
        }

        public static async Task AddCreateAndUpdatePermission(RoleManager<AppRole> roleManager, AppRole role)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Update));
        }

        public static async Task AddDeletePermission(RoleManager<AppRole> roleManager, AppRole role)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Delete));
        }
    }
}
