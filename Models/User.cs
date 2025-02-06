using Newtonsoft.Json;

/********************************************************************************************************************************************
 This file defines the User class, which represents a user in the application.
 Key components and their purposes:
 - Import required namespace for JSON handling.
 - Define the User class with properties to store user information.
 - Use JsonProperty attributes to map class properties to JSON properties.
 - Properties:
   - Id: Unique identifier for the user.
   - Username: Username of the user.
   - PasswordHash: Hashed password of the user.
   - Email: Email address of the user.
   - Type: Type of the user, default is "User".
   - EmailVerified: Indicates whether the user's email is verified.
   - VerificationToken: Token used for email verification.
   - VerificationTokenExpiry: Expiry date and time for the verification token.
**************************************************************************************/

public class User
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; } = "User";

    [JsonProperty(PropertyName = "emailVerified")]
    public bool EmailVerified { get; set; } = false;

    [JsonProperty(PropertyName = "verificationToken")]
    public string? VerificationToken { get; set; }
    [JsonProperty(PropertyName = "verificationTokenExpiry")]
    public DateTime? VerificationTokenExpiry { get; set; } 
}