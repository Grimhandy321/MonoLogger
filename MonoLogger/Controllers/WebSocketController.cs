using Monologer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Monologer.Entities;
using MonoLogger.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Sockets;

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


        [HttpGet("connect")]
        [SwaggerOperation(
            Summary = "Connect to WebSocket server",
            Description = "Initiates the WebSocket handshake and starts receiving messages."
        )]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Connect()
        {
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
                Message message;
                try
                {
                    message = WebSocketController.DeserializedMessage(text);

                }
                catch (Exception e)
                {
                    await SendText(socket, e.Message);
                    continue;
                }

                var user = HttpContext.Items["User"] as User;
                if (user == null)
                {
                    await SendText(socket, "userNotfound");
                    continue;
                }

                message.UserId = user.Id;

                _queue.Queue.Enqueue(message);

                await SendText(socket, "accomplished");
            }

        }

        private static Message DeserializedMessage(string text)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
            var message = JsonSerializer.Deserialize<Message>(text, options);
            if (message == null)
            {
                throw new Exception("invalid_message_format");
            }
            if (message.Text == null)
            {
                throw new Exception("text_cant_be_null");
            }
            return message;
        }

        [HttpPost("message-schema")]
        [Consumes("application/json")]
        public IActionResult MessageTest([FromBody] string text)
        {
            Message message;
            try
            {
                message = WebSocketController.DeserializedMessage(text);

            }
            catch (Exception e)
            {
                return Ok(e.Message);

            }
            return Ok("accomplished");
        }



        private async Task SendText(WebSocket socket, string message)
        {
            var reply = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(reply, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
