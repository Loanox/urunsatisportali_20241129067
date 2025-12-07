using Microsoft.EntityFrameworkCore;
using urunsatisportali.Models;

namespace urunsatisportali.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Elektronik", Description = "Elektronik ürünler", CreatedAt = DateTime.Now },
                new Category { Id = 2, Name = "Giyim", Description = "Giyim ve aksesuar", CreatedAt = DateTime.Now },
                new Category { Id = 3, Name = "Yiyecek & İçecek", Description = "Yiyecek ve içecek ürünleri", CreatedAt = DateTime.Now },
                new Category { Id = 4, Name = "Ev & Bahçe", Description = "Ev ve bahçe ürünleri", CreatedAt = DateTime.Now }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Dizüstü Bilgisayar",
                    Description = "Yüksek performanslı dizüstü bilgisayar",
                    Price = 12999.99m,
                    StockQuantity = 50,
                    SKU = "LAP-001",
                    Brand = "TechMarka",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "Kablosuz Fare",
                    Description = "Ergonomik kablosuz fare",
                    Price = 299.99m,
                    StockQuantity = 200,
                    SKU = "MOU-001",
                    Brand = "TechMarka",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Name = "Pamuklu Tişört",
                    Description = "Rahat pamuklu tişört",
                    Price = 199.99m,
                    StockQuantity = 150,
                    SKU = "TSH-001",
                    Brand = "ModaMarka",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                }
            );

            // Seed Admin User (password: admin123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Yönetici",
                    IsAdmin = true,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}

