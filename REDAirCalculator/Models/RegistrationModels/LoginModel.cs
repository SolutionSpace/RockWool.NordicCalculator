using System.ComponentModel.DataAnnotations;

namespace REDAirCalculator.Models.RegistrationModels
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public string RememberMe { get; set; }
    }
}