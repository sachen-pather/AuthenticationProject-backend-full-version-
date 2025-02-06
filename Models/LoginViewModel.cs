using System.ComponentModel.DataAnnotations;

namespace LoginPage.Models
{

    /********************************************************************************************************************************************
     This file defines the LoginViewModel class, which represents the data required for user login.
     Key components and their purposes:
     - Import required namespace for data annotations.
     - Define the LoginViewModel class with properties to store login information.
     - Properties:
       - Email: Stores the email address of the user. It is required and must be a valid email format.
       - Password: Stores the password of the user. It is required and must be of DataType.Password.
       - RememberMe: Indicates whether the user wants to be remembered on the device.
    ********************************************************************************************************************************************/

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}