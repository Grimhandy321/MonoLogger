using Monologer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Monologer.Entities;
using MonoLogger.Entities;

namespace Monologetr.Controllers
{
    /// <summary>
    /// WebSocket controller for handling real-time messages.
    /// </summary>
    /// <author>Michal Příhoda</author>
    [Route("ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly MessageQueue _queue;

        /// <summary>
        /// Constructor for WebSocketController.
        /// </summary>
        /// <param name="queue">Injected message queue.</param>
        /// <author>Michal Příhoda</author>
        public WebSocketController(MessageQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Handles WebSocket connections and message exchange.
        /// </summary>
        /// <remarks>
        /// Accepts WebSocket requests and processes incoming messages by enqueuing them.
        /// Replies with "accomplished" for each received message.
        /// </remarks>
        /// <author>Michal Příhoda</author>
        [HttpGet]
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var buffer = new byte[4096];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "closed",
                        CancellationToken.None);
                    return;
                }

                var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Message? message = JsonSerializer.Deserialize<Message>(text, options);

                if (message == null)
                {
                    var errorReply = Encoding.UTF8.GetBytes("invalid_message_format");
                    await socket.SendAsync(errorReply, WebSocketMessageType.Text, true, CancellationToken.None);
                    continue;
                }
                var user = HttpContext.Items["User"] as User;

                if (user == null)
                {
                    var errorReply = Encoding.UTF8.GetBytes("unauthorized");
                    await socket.SendAsync(errorReply, WebSocketMessageType.Text, true, CancellationToken.None);
                    continue;
                }
                message.User = user;
                message.UserId = user.Id;

                // Enqueue
                _queue.Queue.Enqueue(message);

                var reply = Encoding.UTF8.GetBytes("accomplished");
                await socket.SendAsync(reply, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
