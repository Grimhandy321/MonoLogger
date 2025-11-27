using Microsoft.EntityFrameworkCore;
using Monologer.Entities;

namespace Monologer.Data
{
    public class DbInsertService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public DbInsertService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        public async Task InsertMessageAsync(string msg)
        {
            using var db = _factory.CreateDbContext();

            var item = new Message
            {
                Text = msg,
                CreatedAt = DateTime.UtcNow
            };

            db.WebSocketMessages.Add(item);
            await db.SaveChangesAsync();
        }
    }
}
