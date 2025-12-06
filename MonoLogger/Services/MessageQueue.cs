using Monologer.Entities;
using System.Collections.Concurrent;

namespace Monologer.Services
{
    public class MessageQueue
    {
        public ConcurrentQueue<Message> Queue { get; } = new();
    }
}
