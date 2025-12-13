using Monologer.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    class MessageFactory
    {
        public static byte[] CreateMessageBuffer( string? text = null,  MessageType type = MessageType.Info,   int magnitude = 1)
        {
            var message = new Message
            {
                Text = text ?? $"hello-{Guid.NewGuid()}",
                Type = type,
                Magnitude = magnitude
            };

            var json = JsonSerializer.Serialize(message);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
