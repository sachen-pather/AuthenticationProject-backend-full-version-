// Import required namespaces for authentication, database access, and JSON handling
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using System.Net;
using Newtonsoft.Json;
using LoginPage;
using LoginPage.Services;

/********************************************************************************************************************************************
 This file configures and builds the web application using the WebApplicationBuilder class.
 It sets up services, middleware, and the HTTP request pipeline for the application.
 Key components and their purposes:
 - Import required namespaces for authentication, database access, and JSON handling.
 - Create a new WebApplicationBuilder instance to configure the application.
 - Add MVC services to enable controller and view functionality.
 - Configure and register the Cosmos DB client as a singleton service for database access.
 - Add and configure the EmailService for sending emails.
 - Configure the authentication system to use cookie-based authentication.
 - Register the password hasher service with scoped lifetime.
 - Configure CORS to allow requests from the React frontend.
 - Build the application using the configured services.
 - Configure the HTTP request pipeline with middleware for error handling, HTTPS redirection, static files, CORS, routing, authentication, and authorization.
 - Initialize and verify the Cosmos DB connection.
 - Configure default routing pattern for the application.
- Start the application.

********************************************************************************************************************************************/

// Create a new WebApplicationBuilder instance which provides the framework for configuring the application
var builder = WebApplication.CreateBuilder(args);

// Add MVC services to enable controller and view functionality
builder.Services.AddControllersWithViews();

// Configure and register the Cosmos DB client as a singleton service
// Singleton ensures the same instance is used throughout the application's lifetime
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    // Get the configuration service from the service provider
    IConfiguration? configuration = sp.GetService<IConfiguration>();
    if (configuration == null)
    {
        throw new InvalidOperationException("Configuration service is not available");
    }

    // Retrieve the Cosmos DB connection string from configuration
    // The ?? operator provides a null-coalescing fallback if the connection string isn't found
    string connectionString = configuration.GetConnectionString("CosmosDb")
        ?? throw new InvalidOperationException("CosmosDB connection string is not configured");

    // Configure Cosmos DB client options for optimal performance and reliability
    var clientOptions = new CosmosClientOptions
    {
        RequestTimeout = TimeSpan.FromSeconds(30),                    // Set maximum time for requests
        ConnectionMode = ConnectionMode.Gateway,                      // Use Gateway mode for connections
        MaxRetryAttemptsOnRateLimitedRequests = 9,                  // Number of retry attempts
        MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30)  // Maximum wait time between retries
    };

    // Create and return a new CosmosClient instance with the specified options
    return new CosmosClient(connectionString, clientOptions);
});

// Add Email Service here
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure the authentication system to use cookie-based authentication
builder.Services.AddAuthentication(options =>
{
    // Set default authentication schemes for different authentication scenarios
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Configure cookie authentication options
    options.LoginPath = "/Account/Login";              // Redirect path when authentication is required
    options.LogoutPath = "/Account/Logout";            // Path for logout functionality
    options.Cookie.HttpOnly = true;                    // Prevent client-side access to the cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Require HTTPS
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);       // Set cookie expiration time
    options.SlidingExpiration = true;                        // Reset expiration on activity
});

// Register the password hasher service with scoped lifetime
// Scoped means a new instance is created for each request
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Configure CORS (Cross-Origin Resource Sharing) to allow requests from the React frontend
// In Program.cs, update your CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", builder =>
    {
        builder.WithOrigins(
                "http://localhost:5173",     // Add this for Vite's default port
                "https://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline with middleware
if (!app.Environment.IsDevelopment())
{
    // In production, use custom error handling
    app.UseExceptionHandler("/Home/Error");
    // Enable HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

// Configure middleware in the correct order
app.UseHttpsRedirection();     // Redirect HTTP requests to HTTPS
app.UseStaticFiles();          // Enable serving static files (CSS, JavaScript, images)
app.UseCors("ReactPolicy");    // Apply CORS policy
app.UseRouting();             // Enable routing
app.UseAuthentication();
app.UseMiddleware<BearerTokenMiddleware>();  // Enable authentication
app.UseAuthorization();        // Enable authorization

// Initialize and verify Cosmos DB connection
try
{
    // Create a service scope to resolve dependencies
    using var scope = app.Services.CreateScope();
    var cosmosClient = scope.ServiceProvider.GetRequiredService<CosmosClient>();

    // Attempt to connect to the database and container
    Database database = cosmosClient.GetDatabase("loginDB");
    Container container = database.GetContainer("Users");

    // Verify the connection by reading container properties
    await container.ReadContainerAsync();
    Console.WriteLine("Successfully connected to existing Cosmos DB container");
}
catch (CosmosException ex)
{
    // Log any Cosmos DB connection errors
    Console.WriteLine($"Cosmos DB Error: {ex.StatusCode} - {ex.Message}");
    throw;
}

// Configure default routing pattern for the application
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // Default route pattern

// Start the application
await app.RunAsync();