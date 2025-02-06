using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp; // Add this for SmtpClient


/********************************************************************************************************************************************
// This file defines the EmailService class and the IEmailService interface for sending verification emails.
// The IEmailService interface declares a method for sending a verification email asynchronously.
// The EmailService class implements the IEmailService interface and provides the functionality to send verification emails using SMTP.
// The EmailService class has the following members:
// - _configuration: An instance of IConfiguration to access app settings.
// - _logger: An instance of ILogger to log information and errors.
// - Constructor: Initializes the _configuration and _logger fields.
// - SendVerificationEmailAsync: Sends a verification email to the specified email address with a verification token.
//   - Retrieves SMTP configuration from app settings.
//   - Constructs the email message with a verification link.
//   - Sends the email using the MailKit SMTP client.
//   - Logs the email sending process and any exceptions that occur.
********************************************************************************************************************************************/

namespace LoginPage.Services
{
    public interface IEmailService
    {
        // Method to send a verification email asynchronously
        Task SendVerificationEmailAsync(string email, string verificationToken);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger; // Add logging

        // Constructor to initialize configuration and logger
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Method to send a verification email
        public async Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            try
            {
                // Log the start of the email sending process
                Console.WriteLine("Starting email sending process");
                Console.WriteLine($"Email configuration: {_configuration["Email:SmtpServer"]}");

                // Retrieve SMTP configuration from app settings
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
                var smtpUsername = _configuration["Email:Username"];
                var fromEmail = _configuration["Email:FromAddress"];

                // Log the email details
                Console.WriteLine($"Attempting to send email to: {email}");
                Console.WriteLine($"Using SMTP server: {smtpServer}");
                Console.WriteLine($"Using port: {smtpPort}");
                Console.WriteLine($"From address: {fromEmail}");

                // Retrieve SMTP password from app settings
                var smtpPassword = _configuration["Email:Password"]
                  ?? throw new InvalidOperationException("SMTP password not configured");
                var appUrl = _configuration["AppUrl"]?.TrimEnd('/');
                var verificationUrl = $"{appUrl}/verify-email?token={verificationToken}";

                // Create the email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Sachen's Project", fromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Verify your email address";

                // Build the email body
                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                        <h2>Welcome to my Project!</h2>
                        <p>Please verify your email address by clicking the link below:</p>
                        <p><a href='{verificationUrl}'>Verify Email Address</a></p>
                        <p>This link will expire in 24 hours.</p>";

                message.Body = builder.ToMessageBody();

                // Send the email using the SMTP client
                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                // Log the successful email sending
                _logger.LogInformation("Verification email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the email sending process
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}