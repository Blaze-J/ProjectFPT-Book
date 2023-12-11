using System.ComponentModel.DataAnnotations;

namespace FPTBook.Models
{
    public class ComparePassValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ChangePass pass = value as ChangePass;
            if (pass.NewPass != pass.ConfirmNewPass)
            {
                ErrorMessage = "ConfirmPass is not same";
                return false;
            }
            return true;
        }
    }
}
