namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class AssignRoleToUserViewModel
    {
        public string RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public bool Exist { get; set; }
    }
}
