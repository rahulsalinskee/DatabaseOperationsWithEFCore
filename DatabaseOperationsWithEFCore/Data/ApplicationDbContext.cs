using Microsoft.EntityFrameworkCore;

namespace DatabaseOperationsWithEFCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
    }
}
