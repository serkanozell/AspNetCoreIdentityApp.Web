using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "User name can't be null")]
        [Display(Name = "UserName :")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "E-Mail Format error")]
        [Required(ErrorMessage = "E-Mail can't be null")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone can't be null")]
        [Display(Name = "Phone Number :")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password can't be null")]
        [Display(Name = "Password :")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Password and Password Confirm fields must be same.")]
        [Required(ErrorMessage = "Password Confirm can't be null")]
        [Display(Name = "Password Confirm :")]
        public string PasswordConfirm { get; set; }

        public SignUpViewModel()
        {
        }

        public SignUpViewModel(string userName, string email, string phoneNumber, string password, string passwordConfirm)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            PasswordConfirm = passwordConfirm;
        }
    }
}
