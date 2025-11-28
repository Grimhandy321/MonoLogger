using Microsoft.EntityFrameworkCore;
using Monologer.Data;
using Monologer.Services;

namespace MonoLogger
{
    /// <summary>
    /// Builds and runs the MonoLogger web application.
    /// </summary>
    /// <author>Michal Pøíhoda</author>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // database connection
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            // Services
            builder.Services.AddSingleton<MessageQueue>();
            builder.Services.AddSingleton<DbInsertService>();
            builder.Services.AddSingleton<WorkerPool>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseWebSockets(); // necessary for WebSocket support (not default ins Asp.Net )
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Runs the worker pool 
            var workers = app.Services.GetRequiredService<WorkerPool>();
            workers.Start();

            app.Run();
        }
    }
}
