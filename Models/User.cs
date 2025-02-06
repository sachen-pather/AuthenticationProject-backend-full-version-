using Newtonsoft.Json;

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