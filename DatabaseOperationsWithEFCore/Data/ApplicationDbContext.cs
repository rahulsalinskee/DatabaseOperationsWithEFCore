using DatabaseOperationsWithEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseOperationsWithEFCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Language> Languages { get; set; }
    }
}
