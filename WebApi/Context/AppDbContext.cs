using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tile> Tiles { get; set; } = null!;
        public DbSet<Server> Servers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tile>()
                .HasOne(t => t.Server)
                .WithMany(s => s.Tiles)
                .HasForeignKey(t => t.ServerId)
                .IsRequired();

            modelBuilder.Entity<Tile>()
                .HasIndex(t => new { t.ServerId, t.X, t.Y, t.Type, t.Status });
        }
    }
}