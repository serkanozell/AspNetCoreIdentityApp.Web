using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password can't be null")]
        [Display(Name = "Old Password :")]
        [MinLength(6, ErrorMessage = "Old Password must be contains more than 6 character")]
        public string PasswordOld { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password can't be null")]
        [Display(Name = "New Password :")]
        [MinLength(6, ErrorMessage = "New Password must be contains more than 6 character")]
        public string PasswordNew { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(PasswordNew), ErrorMessage = "Password and Password Confirm fields must be same.")]
        [Required(ErrorMessage = "Password Confirm can't be null")]
        [Display(Name = "New Password Confirm :")]
        [MinLength(6, ErrorMessage = "New Password Confirm must be contains more than 6 character")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
