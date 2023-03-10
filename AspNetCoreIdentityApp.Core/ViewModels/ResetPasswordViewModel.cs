using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password can't be null")]
        [Display(Name = "New Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and Password Confirm fields must be same.")]
        [Required(ErrorMessage = "Password Confirm can't be null")]
        [Display(Name = "New Password Confirm")]
        public string? PasswordConfirm { get; set; }
    }
}
