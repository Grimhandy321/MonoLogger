using Microsoft.Extensions.Configuration;
using Monologer.Data;

namespace Monologer.Services
{
    /// <summary>
    /// A pool of worker threads that process messages from a queue
    /// and insert them into the database. All threads read from the same queue in oreder to balance the load.
    /// </summary>
    /// <author>Michal Příhoda</author>
    public class WorkerPool
    {
        private readonly MessageQueue _queue;
        private readonly DbInsertService _db;
        private readonly int _maxThreads;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerPool"/> class.
        /// </summary>
        /// <param name="queue">The message queue to process.</param>
        /// <param name="db">Database insertion service.</param>
        /// <param name="config">Configuration for setting maximum number of threads.</param>
        /// <author>Michal Příhoda</author>
        public WorkerPool(MessageQueue queue, DbInsertService db, IConfiguration config)
        {
            _queue = queue;
            _db = db;
            _maxThreads = int.Parse(config["WorkerPool:MaxThreads"] ?? "4");
        }

        /// <summary>
        /// Starts the worker pool, creating background threads to process the queue.
        /// </summary>
        public void Start()
        {
            for (int i = 0; i < _maxThreads; i++)
            {
                var thread = new Thread(async () => await WorkerLoop())
                {
                    IsBackground = false,
                    Name = $"worker-{i}"
                };
                thread.Start();
            }
        }

        /// <summary>
        /// The main loop executed by each worker thread.
        /// Dequeues messages from the queue and inserts them into the database.
        /// </summary>
        /// <remarks>
        /// Runs indefinitely in a background thread. Sleeps briefly when the queue is empty.
        /// </remarks>
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
