using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string RoleId { get; set; } = null!;


        [Required(ErrorMessage = "Role name can't be null")]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
    }
}
