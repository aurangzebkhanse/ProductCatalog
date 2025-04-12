using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial data (optional)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "A powerful laptop", Price = 1000.99m, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Description = "A sleek smartphone", Price = 799.49m, Stock = 20 }
            );

        }
    }
}
