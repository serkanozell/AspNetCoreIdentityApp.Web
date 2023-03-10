using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
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

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password can't be null")]
        [Display(Name = "Password :")]
        [MinLength(6, ErrorMessage = "Password must be contains more than 6 character")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and Password Confirm fields must be same.")]
        [Required(ErrorMessage = "Password Confirm can't be null")]
        [Display(Name = "Password Confirm :")]
        [MinLength(6, ErrorMessage = "Password must be contains more than 6 character")]
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
