using Monologer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Monologer.Entities;
using MonoLogger.Entities;
using Swashbuckle.AspNetCore.Annotations;

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

        public WebSocketController(MessageQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Initiates a WebSocket upgrade and starts the real-time messaging loop.
        /// </summary>
        /// <remarks>
        /// This endpoint appears in Swagger and can be called normally.
        /// If the request is a WebSocket upgrade request, the connection will be upgraded.
        /// </remarks>
        /// <returns>A status message or an upgraded WebSocket session.</returns>
        [HttpGet("connect")]
        [SwaggerOperation(
            Summary = "Connect to WebSocket server",
            Description = "Initiates the WebSocket handshake and starts receiving messages."
        )]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Connect()
        {
            // If not a WebSocket request (normal HTTP) → show info in Swagger
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return Ok("WebSocket endpoint ready. Use ws://yourserver/ws/connect");
            }

            // Handle websocket
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await HandleWebSocket(socket);
            return new EmptyResult();
        }

        /// <summary>
        /// Handles WebSocket message loop.
        /// </summary>
        private async Task HandleWebSocket(WebSocket socket)
        {
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
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var message = new Message();
                try
                {
                    message = JsonSerializer.Deserialize<Message>(text, options);

                }
                catch {
                    await SendText(socket, "invalid_message_format");
                    continue;
                }
              

                if (message == null)
                {
                    await SendText(socket, "invalid_message_format");
                    continue;
                }

                var user = HttpContext.Items["User"] as User;
                if (user == null)
                {
                    await SendText(socket, "unauthorized");
                    continue;
                }

                message.User = user;
                message.UserId = user.Id;

                _queue.Queue.Enqueue(message);

                await SendText(socket, "accomplished");
            }
        }

        private async Task SendText(WebSocket socket, string message)
        {
            var reply = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(reply, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
