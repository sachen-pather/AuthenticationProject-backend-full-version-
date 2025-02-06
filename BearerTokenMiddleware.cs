namespace LoginPage
{
    public class BearerTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public BearerTokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Print the current request path for debugging
            Console.WriteLine($"Current request path: {context.Request.Path}");

            var publicPaths = new[]
            {
            "/account/verify-email",  // Make paths lowercase
            "/account/login",
            "/account/register",
            "/"  // Add root path if needed
        };

            // Debug print
            Console.WriteLine($"Checking if path matches any public paths");

            var currentPath = context.Request.Path.ToString().ToLowerInvariant();

            // Check if it's a public path
            if (publicPaths.Any(p => currentPath.StartsWith(p)))
            {
                Console.WriteLine($"Matched public path: {currentPath}");
                await _next(context);
                return;
            }

            Console.WriteLine($"No public path match found, checking bearer token");

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var expectedToken = $"Bearer {_configuration["BearerToken"]}";

            if (string.IsNullOrEmpty(token) || token != expectedToken.Split(" ").Last())
            {
                Console.WriteLine("Token validation failed");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Bearer token is required!");
                return;
            }

            await _next(context);
        }
    }
}
