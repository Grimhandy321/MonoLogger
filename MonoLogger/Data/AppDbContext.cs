using Monologer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Monologer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Message> WebSocketMessages => Set<Message>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
