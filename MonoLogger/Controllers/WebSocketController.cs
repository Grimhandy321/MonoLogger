using Monologer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;


namespace Monologetr.Controllers
{
    [Route("ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly MessageQueue _queue;

        public WebSocketController(MessageQueue queue)
        {
            _queue = queue;
        }

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


                _queue.Queue.Enqueue(text);

                var reply = Encoding.UTF8.GetBytes("accomplished");
                await socket.SendAsync(reply, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
