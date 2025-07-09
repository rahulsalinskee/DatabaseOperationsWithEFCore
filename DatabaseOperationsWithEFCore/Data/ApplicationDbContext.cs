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

        public DbSet<BookPrice> BookPrices { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Master Table */
            modelBuilder.Entity<Currency>().HasData(
                new Currency() { Id = 1, Title = "USD", Description = "United States Dollar", },
                new Currency() { Id = 2, Title = "EUR", Description = "Euro", },
                new Currency() { Id = 3, Title = "GBP", Description = "British Pound Sterling", },
                new Currency() { Id = 4, Title = "INR", Description = "Indian Rupee", },
                new Currency() { Id = 5, Title = "JPY", Description = "Japanese Yen", },
                new Currency() { Id = 6, Title = "CNY", Description = "Chinese Yuan", },
                new Currency() { Id = 7, Title = "AUD", Description = "Australian Dollar", },
                new Currency() { Id = 8, Title = "CAD", Description = "Canadian Dollar", },
                new Currency() { Id = 9, Title = "CHF", Description = "Swiss Franc", },
                new Currency() { Id = 10, Title = "RUB", Description = "Russian Ruble", }
            );
        }
    }
}
