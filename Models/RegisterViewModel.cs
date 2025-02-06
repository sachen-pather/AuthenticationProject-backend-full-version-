using System.ComponentModel.DataAnnotations;

/********************************************************************************************************************************************
 This file defines the RegisterViewModel class, which represents the data required for user registration.
 Key components and their purposes:
 - Import required namespace for data annotations.
 - Define the RegisterViewModel class with properties to store registration information.
 - Properties:
   - Email: Stores the email address of the user. It is required and must be a valid email format.
   - Password: Stores the password of the user. It is required and must have a minimum length of 6 characters.
   - ConfirmPassword: Stores the confirmation password. It is required and must match the Password property.
********************************************************************************************************************************************/

namespace LoginPage.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
