using Airbnb.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Db
{
    public class BookingContext:DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Stay> Stays { get; set; }
        //public DbSet<StayHost> StayHosts { get; set; }
        public DbSet<Loc> Locs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StatReviews> StatReviews { get; set; }
        public DbSet<StayImage> StayImages { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<LikedByUser> LikedByUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Guest> Guests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stay → Host (many-to-one)
            modelBuilder.Entity<Stay>()
                .HasOne(s => s.Host)
                .WithMany(h => h.Stays)
                .HasForeignKey(s => s.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Stay → Loc (one-to-one)
            modelBuilder.Entity<Stay>()
                .HasOne(s => s.Loc)
                .WithOne(l => l.Stay)
                .HasForeignKey<Loc>(l => l.StayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Stay → Reviews (One-to-Many)
            modelBuilder.Entity<Stay>()
                .HasMany(s => s.Reviews)
                .WithOne(r => r.Stay)
                .HasForeignKey(r => r.StayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reviews → StatReviews (One-to-One)
            modelBuilder.Entity<StatReviews>()
                .HasOne(sr => sr.Review)
                .WithOne(r => r.StatReviews)
                .HasForeignKey<StatReviews>(sr => sr.ReviewId)
                .OnDelete(DeleteBehavior.Restrict);


            // Order → Buyer (many-to-one with User)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(u=>u.Orders)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Stay (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Stay)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Host (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Host)
                .WithMany()
                .HasForeignKey(o => o.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Guests (one-to-one)
            modelBuilder.Entity<Order>()
                 .HasOne(o => o.Guests)
                 .WithOne(g => g.Order)
                 .HasForeignKey<Guest>(g => g.OrderId)
                 .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Stay>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

        }
    }
}

