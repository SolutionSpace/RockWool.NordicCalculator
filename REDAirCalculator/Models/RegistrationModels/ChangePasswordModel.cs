using System.ComponentModel.DataAnnotations;

namespace REDAirCalculator.Models.RegistrationModels
{
    public class ChangePasswordModel
    {
        [Display(Name = "MemberGuId")]
        [Required]
        public string MemberGuId { get; set; }

        [Display(Name = "NewPassword")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "ConfirmNewPassword")]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}