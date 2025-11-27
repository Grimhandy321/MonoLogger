using Microsoft.EntityFrameworkCore;
using Monologer.Data;
using Monologer.Services;

namespace MonoLogger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();



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
            app.UseWebSockets();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            var workers = app.Services.GetRequiredService<WorkerPool>();
            workers.Start();

            app.Run();
        }
    }
}
