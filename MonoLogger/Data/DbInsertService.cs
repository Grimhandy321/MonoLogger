using Microsoft.EntityFrameworkCore;
using Monologer.Entities;

namespace Monologer.Data
{
    /// <summary>
    /// Service for inserting messages into the database.
    /// </summary>
    /// <author>Michal Příhoda</author>
    public class DbInsertService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        /// <summary>
        /// Constructor for DbInsertService.
        /// </summary>
        /// <param name="factory">Factory for creating AppDbContext instances.</param>
        public DbInsertService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Inserts a message into the database with the current UTC timestamp.
        /// </summary>
        /// <param name="msg">The message text to insert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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
