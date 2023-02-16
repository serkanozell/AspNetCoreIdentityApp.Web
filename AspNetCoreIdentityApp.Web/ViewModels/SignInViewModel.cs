using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {
        [EmailAddress(ErrorMessage = "E-Mail Format error")]
        [Required(ErrorMessage = "E-Mail can't be null")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password can't be null")]
        [Display(Name = "Password :")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public SignInViewModel()
        {
        }

        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
