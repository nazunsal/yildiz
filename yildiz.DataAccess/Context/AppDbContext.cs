using Microsoft.EntityFrameworkCore;
using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                CategoryId = 1,
                CategoryName = "Genel"
            }
        );

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                UserId = 1,
                Name = "Yıldız Admin",
                Username = "admin",
                Email = "admin@yildiz.com",
                Password = "1234",
                IsAdmin = true
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}