using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SlicaWeb.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string APIKey { get; set; }

        public string SessionKey { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is Mandatory")]

        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is Mandatory")]

        public string LastName { get; set; }
        public string FileName { get; set; }

        public string Bio { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is Mandatory")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\-])(?=.*[0-9]).{6,18}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one special character, and one numeric digit.")]

        public string Password { get; set; }


        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        [Required(ErrorMessage = "Confirm Password is Mandatory")]

        public string ConfirmPassword { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Old Password is Mandatory")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\-])(?=.*[0-9]).{6,18}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one special character, and one numeric digit.")]


        public string OldPassword { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is Mandatory")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }
        [DisplayName("I agree to the terms of use and privacy policy.")]
        [MustBeTrue(ErrorMessage = "Please agree to the terms of use and privacy policy.")]

        public bool IsAgreePolicy { get; set; }
        public bool IsTwoFactor { get; set; }


        public bool IsEmailVerified { get; set; }

    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }

    }
}
