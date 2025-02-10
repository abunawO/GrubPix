using GrubPix.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrubPix.Infrastructure.Persistence
{
    public class GrubPixDbContext : DbContext
    {
        public GrubPixDbContext(DbContextOptions<GrubPixDbContext> options) : base(options) { }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Restaurant - Menu Relationship (One-to-Many)
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Menus)
                .WithOne(m => m.Restaurant)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Menu - MenuItem Relationship (One-to-Many)
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.MenuItems)
                .WithOne(mi => mi.Menu)
                .HasForeignKey(mi => mi.MenuId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure this line exists
        }
    }
}
