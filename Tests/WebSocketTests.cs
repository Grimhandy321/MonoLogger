using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class WebSocketIntegrationTests
    {
        private readonly string _url = "ws://localhost:5151/ws";

        [Fact]
        public async Task WebSocket_ShouldConnect_SendMessage_ReceiveResponse()
        {
            using var ws = new ClientWebSocket();

            await ws.ConnectAsync(new Uri(_url), CancellationToken.None);
            Assert.Equal(WebSocketState.Open, ws.State);
            string msg = "test-message";
            byte[] sendBuffer = Encoding.UTF8.GetBytes(msg);
            await ws.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            var receiveBuffer = new byte[4096];
            var result = await ws.ReceiveAsync(receiveBuffer, CancellationToken.None);

            string reply = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            Assert.Contains("accomplished", reply);

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
        }

        [Fact]
        public async Task WebSocket_ShouldHandleMultipleConnections()
        {
            int connectionCount = 5;

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < connectionCount; i++)
            {
                int id = i;
                tasks.Add(Task.Run(async () =>
                {
                    using var ws = new ClientWebSocket();
                    await ws.ConnectAsync(new Uri(_url), CancellationToken.None);
                    Assert.Equal(WebSocketState.Open, ws.State);

                    string msg = $"hello-{id}";
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                    var recv = new byte[4096];
                    var result = await ws.ReceiveAsync(recv, CancellationToken.None);
                    string reply = Encoding.UTF8.GetString(recv, 0, result.Count);

                    Assert.Contains("accomplished", reply);

                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
