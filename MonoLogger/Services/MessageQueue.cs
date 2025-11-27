using System.Collections.Concurrent;

namespace Monologer.Services
{
    public class MessageQueue
    {
        public ConcurrentQueue<string> Queue { get; } = new();
    }
}
