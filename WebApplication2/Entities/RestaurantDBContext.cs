using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Entities;

public class RestaurantDbContext : DbContext
{
    private readonly IConfiguration _configuration; //Getting access to config class
    
    public RestaurantDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
    }
    
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>()
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(25);

        modelBuilder.Entity<Dish>()
            .Property(d => d.Name)
            .IsRequired();

        modelBuilder.Entity<Address>()
            .Property(a => a.City)
            .HasMaxLength(50);

        modelBuilder.Entity<Address>()
            .Property(a => a.Street)
            .HasMaxLength(50);
    }
}