using ConFlux.Model;
using ConFlux.Model.YourApp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ConFlux.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ObjectType> ObjectTypes { get; set; }
        public DbSet<QuarterPeriod> QuarterPeriods { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<PriceItem> PriceItems { get; set; }

        public DbSet<PriceType> PriceTypes { get; set; }

        public DbSet<UserPriceRequestLog> UserPriceRequestLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);


        }
    }
}