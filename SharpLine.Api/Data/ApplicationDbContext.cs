using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Models;

namespace SharpLine.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public DbSet<Shop> Shops { get; set; } = null!;
        public DbSet<Barber> Barbers { get; set; } = null!;
        public DbSet<Availability> Availabilities { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Shop-Barber relationship
            builder.Entity<Barber>()
                   .HasOne(b => b.Shop)
                   .WithMany(s => s.Barbers)
                   .HasForeignKey(b => b.ShopId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Barber-Availability relationship
            builder.Entity<Availability>()
                   .HasOne(a => a.Barber)
                   .WithMany(b => b.Availabilities)
                   .HasForeignKey(a => a.BarberId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Barber-Booking relationship
            builder.Entity<Booking>()
                   .HasOne(b => b.Barber)
                   .WithMany(barber => barber.Bookings)
                   .HasForeignKey(b => b.BarberId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Customer-Booking relationship
            builder.Entity<Booking>()
                   .HasOne(b => b.Customer)
                   .WithMany()
                   .HasForeignKey(b => b.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
