using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class UserEditViewModel
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

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date :")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "City :")]
        public string? City { get; set; }

        [Display(Name = "Profie Picture :")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Gender  :")]
        public Gender? Gender { get; set; }
    }
}
