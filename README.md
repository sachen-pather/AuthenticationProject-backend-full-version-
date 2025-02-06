# Secure Authentication API with Email Verification

## Overview
This .NET Web API implements secure user authentication with email verification capabilities. Built using ASP.NET Core, the system provides robust user management with features including registration, login, and email verification flows. It utilizes Cosmos DB for data storage and Gmail SMTP services for sending verification emails.

A demonstration of the system can be seen in the images below.

![1000146812](https://github.com/user-attachments/assets/c59f7d3a-cf8d-4687-ba0c-cfba86ca3496)
![Screenshot_2025-02-06-15-52-04-238_com miui videoplayer](https://github.com/user-attachments/assets/8ed167cc-7210-422e-8040-1efcaf29e5a4)
![Screenshot_2025-02-06-15-52-13-978_com miui videoplayer](https://github.com/user-attachments/assets/daf7da9c-9c43-4620-ac23-3a08871b5921)
![Screenshot_2025-02-06-15-52-18-439_com miui videoplayer](https://github.com/user-attachments/assets/51850512-0389-40b0-b1e6-6e8218dde8c5)
![Screenshot_2025-02-06-16-02-36-893_com brave browser](https://github.com/user-attachments/assets/3f80e243-8f03-4c7c-9bcc-fc413cde0ab3)
![Screenshot_2025-02-06-15-52-48-526_com miui videoplayer](https://github.com/user-attachments/assets/aed16e34-2f76-4ac5-a58b-23170f071cf8)


## Features
* User registration with email verification
* Secure login with password hashing
* Cookie-based authentication system
* Token-based API security
* Cosmos DB integration for user management
* Email verification using Gmail SMTP
* CORS support for frontend integration

## Technical Stack
* ASP.NET Core 7.0
* Azure Cosmos DB
* MailKit for email services
* Microsoft Identity for password handling
* Gmail SMTP for email delivery

## File Structure
```
LoginPage/
├── Controllers/
│   └── AccountController.cs        # Handles authentication endpoints
├── Models/
│   ├── User.cs                     # User data model
│   ├── LoginViewModel.cs           # Login request model
│   └── RegisterViewModel.cs        # Registration request model
├── Services/
│   ├── EmailService.cs             # Email service implementation
│   └── IEmailService.cs            # Email service interface
├── Middleware/
│   └── BearerTokenMiddleware.cs    # Token validation middleware
├── Properties/
│   └── launchSettings.json         # Launch configuration
├── appsettings.json               # Application settings
├── appsettings.Development.json   # Development settings
├── Program.cs                     # Application entry point
└── LoginPage.csproj               # Project file
```

## Configuration Setup
### appsettings.json Template
```json
{
    "ConnectionStrings": {
        "CosmosDb": "your-cosmos-connection-string"
    },
    "BearerToken": "your-bearer-token",
    "Email": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 587,
        "Username": "your-gmail@gmail.com",
        "Password": "your-16-digit-app-password",
        "FromAddress": "your-gmail@gmail.com"
    },
    "AppUrl": "http://localhost:5173",
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "Authentication": {
        "Schemes": {
            "Cookies": {
                "ValidateInterval": "30m"
            }
        }
    }
}
```

## Prerequisites
* .NET 7.0 or later
* Azure Cosmos DB account
* Gmail account with:
  * 2-Step Verification enabled
  * App Password generated
* Visual Studio 2022 or VS Code

## Installation Steps
1. Clone the repository
2. Configure appsettings.Development.json with your credentials
3. Install required NuGet package:
```bash
dotnet add package MailKit
```
4. Run the application:
```bash
dotnet run
```

## API Endpoints
### Authentication Endpoints
* POST `/Account/register` - New user registration
* POST `/Account/login` - User login
* GET `/Account/verify-email` - Email verification
* GET `/Account/logout` - User logout

## Email Configuration
1. Enable 2-Step Verification in Google Account
2. Generate App Password:
   * Go to Google Account Security
   * Find App Passwords under 2-Step Verification
   * Generate new password for Mail
3. Use generated password in email configuration

## Security Considerations
* Never commit sensitive configuration values
* Use environment variables in production
* Secure bearer tokens
* Implement HTTPS in production
* Rotate email app passwords regularly

## Frontend Integration
* Default development URL: http://localhost:5173
* Configure CORS in Program.cs for additional origins
* Ensure frontend uses correct API endpoints
* Implement proper error handling

## Error Handling
* Email service failures
* Database connection issues
* Token validation errors
* User verification states

## Development Notes
* Use appsettings.Development.json for local development
* Keep bearer token middleware configured
* Monitor email delivery in development
* Test verification flow thoroughly

## Production Deployment
* Set up proper environment variables
* Configure production database connection
* Set up proper SMTP credentials
* Enable appropriate security measures
* Configure proper CORS settings

## Maintenance
* Monitor database performance
* Check email delivery statistics
* Update security configurations
* Maintain user verification status

For additional support or questions, contact 0812354879
