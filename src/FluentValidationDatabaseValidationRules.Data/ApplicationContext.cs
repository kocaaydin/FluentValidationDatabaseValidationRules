using FluentValidationDatabaseValidationRules.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace FluentValidationDatabaseValidationRules.Data
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                        .Property(b => b.Name)
                        .HasMaxLength(10)
                        .IsRequired();

            modelBuilder.Entity<Product>()
                        .Property(x => x.Number)
                        .HasColumnType("decimal(5, 3)");
        }

        public DbSet<Product> Products { get; set; }

    }
}
