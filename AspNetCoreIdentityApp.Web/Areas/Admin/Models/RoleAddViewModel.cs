using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleAddViewModel
    {
        [Required(ErrorMessage = "Role name can't be null")]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
    }
}
