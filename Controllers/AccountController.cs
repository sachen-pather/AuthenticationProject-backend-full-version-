// Required namespaces for authentication, identity management, and database operations
using LoginPage.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Net;
using LoginPage.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace LoginPage.Controllers
{
    /// <summary>
    /// Controller responsible for handling all authentication-related operations
    /// including user login, registration, and logout processes.
    /// </summary>
    [ApiController]  // Enables API-specific behaviors
    [Route("[controller]")]  // Sets up routing based on controller name
    public class AccountController : Controller
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;  

        public AccountController(
            CosmosClient cosmosClient,
            IPasswordHasher<User> passwordHasher,
            IEmailService emailService,
            IConfiguration configuration)  
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetDatabase("loginDB").GetContainer("Users");
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _configuration = configuration;  
        }

        /// <summary>
        /// Handles GET requests to the login endpoint
        /// Returns the login view for user authentication
        /// </summary>
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(); //returns view of login interface
        }

        /// <summary>
        /// Processes login attempts and manages user authentication
        /// </summary>
        /// <param name="model">Contains user login credentials</param>
        /// <returns>Authentication result with appropriate status code</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var queryDefinition = new QueryDefinition(
                        "SELECT * FROM c WHERE c.email = @email")
                        .WithParameter("@email", model.Email);
                    //by using @ we can pass the value of email to the query without sql injections being a threat (paramterized query)
                    var query = _container.GetItemQueryIterator<User>(queryDefinition);
                    var user = (await query.ReadNextAsync()).FirstOrDefault();

                    if (user != null)
                    {
                        // Check email verification first
                        if (!user.EmailVerified)
                        {
                            return BadRequest(new { message = "Please verify your email before logging in." });
                        }

                        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                        if (result == PasswordVerificationResult.Success)
                        {
                            // Rest of your successful login code
                            return Ok(new { message = "Login successful" });
                        }
                    }
                    return BadRequest(new { message = "Invalid login attempt." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Login error: {ex.Message}");
                    return StatusCode(500, new { message = "An error occurred during login." });
                }
            }
            return BadRequest(ModelState);
        }
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                // Decode the token
                token = WebUtility.UrlDecode(token);
                Console.WriteLine($"Received token: {token}");

                var queryDefinition = new QueryDefinition(
                    "SELECT * FROM c WHERE c.verificationToken = @token")
                    .WithParameter("@token", token);

                var query = _container.GetItemQueryIterator<User>(queryDefinition);
                var user = (await query.ReadNextAsync()).FirstOrDefault();

                if (user == null || user.VerificationTokenExpiry < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Invalid or expired verification token." });
                }

                // Update user to verified status
                user.EmailVerified = true;
                user.VerificationToken = null;
                user.VerificationTokenExpiry = null;

                await _container.ReplaceItemAsync(
                    user,
                    user.Id,
                    new PartitionKey(user.Type)
                );

                return Ok(new { message = "Email verified successfully. You can now log in." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Verification error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while verifying email." });
            }
        }
        /// <summary>
        /// Handles new user registration
        /// </summary>
        /// <param name="model">Contains new user registration data</param>
        /// <returns>Registration result with appropriate status code</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                // Debug logging
                Console.WriteLine("Starting registration process");
                Console.WriteLine($"Email config: {_configuration["Email:SmtpServer"]}");

                var verificationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    Username = model.Email,
                    PasswordHash = _passwordHasher.HashPassword(null, model.Password),
                    Type = "User",
                    EmailVerified = false,
                    VerificationToken = verificationToken,
                    VerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
                };

                Console.WriteLine("Attempting to save user to database");
                await _container.CreateItemAsync(user, new PartitionKey(user.Type));
                Console.WriteLine("User saved successfully");

                Console.WriteLine("Attempting to send verification email");
                await _emailService.SendVerificationEmailAsync(user.Email, verificationToken);
                Console.WriteLine("Email sent successfully");

                return Ok(new { message = "Registration successful! Please check your email to verify your account." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Handles user logout process
        /// </summary>
        /// <returns>Logout result with appropriate status code</returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            // Sign out user and clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
//structure