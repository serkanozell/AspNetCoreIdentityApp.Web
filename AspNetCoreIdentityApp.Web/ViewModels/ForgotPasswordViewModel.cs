using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "E-Mail Format error")]
        [Required(ErrorMessage = "E-Mail can't be null")]
        [Display(Name = "Email :")]
        public string Email { get; set; }
    }
}
