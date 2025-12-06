using Monologer.Entities;
using Microsoft.EntityFrameworkCore;
using MonoLogger.Entities;

namespace Monologer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Message> WebSocketMessages => Set<Message>();
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
