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
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FavoriteMenuItem> FavoriteMenuItems { get; set; }
        public DbSet<MenuItemImage> MenuItemImages { get; set; }

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

            //MenuItem - MenuItemImages Relationship (One-to-Many) [NEWLY ADDED]
            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.Images)
                .WithOne() // No navigation back to MenuItem to avoid cyclic dependency
                .HasForeignKey(img => img.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Restaurant Relationship (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Restaurants) // A User can have many Restaurants
                .WithOne(r => r.Owner) // A Restaurant belongs to one User
                .HasForeignKey(r => r.OwnerId) // Foreign key in the Restaurant entity
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if User is deleted

            // Define relationships
            modelBuilder.Entity<FavoriteMenuItem>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.FavoriteMenuItems)
                .HasForeignKey(f => f.CustomerId);

            modelBuilder.Entity<FavoriteMenuItem>()
                .HasOne(f => f.MenuItem)
                .WithMany()
                .HasForeignKey(f => f.MenuItemId);

        }
    }
}
