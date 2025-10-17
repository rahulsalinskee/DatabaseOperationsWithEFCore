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

        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Master Table - Currency */
            modelBuilder.Entity<Currency>().HasData
            (
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

            /* Master Table - Language */
            modelBuilder.Entity<Language>().HasData
            (
                new Language() { Id = 1, Title = "English", Description = "English Language" },
                new Language() { Id = 2, Title = "Spanish", Description = "Spanish Language" },
                new Language() { Id = 3, Title = "French", Description = "French Language" },
                new Language() { Id = 4, Title = "German", Description = "German Language" },
                new Language() { Id = 5, Title = "Chinese", Description = "Chinese Language" },
                new Language() { Id = 6, Title = "Japanese", Description = "Japanese Language" },
                new Language() { Id = 7, Title = "Russian", Description = "Russian Language" },
                new Language() { Id = 8, Title = "Hindi", Description = "Hindi Language" },
                new Language() { Id = 9, Title = "Sanskrit", Description = "Sanskrit Language" },
                new Language() { Id = 10, Title = "Portuguese", Description = "Portuguese Language" }
            );
        }
    }
}
