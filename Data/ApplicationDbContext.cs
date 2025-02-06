using Microsoft.EntityFrameworkCore;

namespace LoginPage.Data
{
    /********************************************************************************************************************************************
     This file defines the ApplicationDbContext class, which represents the Entity Framework Core database context for the application.
     Key components and their purposes:
     - Import required namespace for Entity Framework Core.
     - Define the ApplicationDbContext class, inheriting from DbContext.
     - Constructor: Initializes the ApplicationDbContext with the specified options.
     - Properties:
       - Users: A DbSet<User> representing the Users table in the database.
    ********************************************************************************************************************************************/

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
