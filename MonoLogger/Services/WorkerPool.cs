using Microsoft.Extensions.Configuration;
using Monologer.Data;

namespace Monologer.Services
{
    public class WorkerPool
    {
        private readonly MessageQueue _queue;
        private readonly DbInsertService _db;
        private readonly int _maxThreads;

        public WorkerPool(MessageQueue queue, DbInsertService db, IConfiguration config)
        {
            _queue = queue;
            _db = db;
            _maxThreads = int.Parse(config["WorkerPool:MaxThreads"] ?? "4");
        }

        public void Start()
        {
            for (int i = 0; i < _maxThreads; i++)
            {
                var thread = new Thread(async () => await WorkerLoop())
                {
                    IsBackground = true,// will run if the main program exits
                    Name = $"worker-{i}"
                };
                thread.Start();
            }
        }

        private async Task WorkerLoop()
        {
            while (true)
            {
                if (_queue.Queue.TryDequeue(out var message))
                {
                    try
                    {
                        await _db.InsertMessageAsync(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DB ERROR: {ex.Message}");
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }
    }
}
