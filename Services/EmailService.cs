using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp; // Add this for SmtpClient

namespace LoginPage.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationToken);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger; // Add logging

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            try
            {
                Console.WriteLine("Starting email sending process");
                Console.WriteLine($"Email configuration: {_configuration["Email:SmtpServer"]}");

                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
                var smtpUsername = _configuration["Email:Username"];
                var fromEmail = _configuration["Email:FromAddress"];

                Console.WriteLine($"Attempting to send email to: {email}");
                Console.WriteLine($"Using SMTP server: {smtpServer}");
                Console.WriteLine($"Using port: {smtpPort}");
                Console.WriteLine($"From address: {fromEmail}");


                var smtpPassword = _configuration["Email:Password"]
                  ?? throw new InvalidOperationException("SMTP password not configured");
                var appUrl = _configuration["AppUrl"]?.TrimEnd('/');
                var verificationUrl = $"{appUrl}/verify-email?token={verificationToken}";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Sachen's Project", fromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Verify your email address";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                    <h2>Welcome to my Project!</h2>
                    <p>Please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationUrl}'>Verify Email Address</a></p>
                    <p>This link will expire in 24 hours.</p>";

                message.Body = builder.ToMessageBody();

                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Verification email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}