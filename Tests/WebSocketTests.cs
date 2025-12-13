using Microsoft.Extensions.Configuration;
using Monologer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Helpers;

namespace Tests
{
    public class WebSocketIntegrationTests : AbstractTest
    {
        [Fact]
        public async Task WebSocket_ShouldConnect_SendMessage_ReceiveResponse()
        {
            using var ws = new ClientWebSocket();
            ws.Options.SetRequestHeader("Authorization", _token);

            await ws.ConnectAsync(new Uri(_url), CancellationToken.None);

            Assert.Equal(WebSocketState.Open, ws.State);

            byte[] buffer = MessageFactory.CreateMessageBuffer("testMessage");

            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer = new byte[4096];
            var result = await ws.ReceiveAsync(receiveBuffer, CancellationToken.None);

            string reply = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            Assert.Equal("accomplished", reply);

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
        }

        [Fact]
        public async Task WebSocket_ShouldHandleMultipleConnections()
        {
            const int connectionCount = 5;
            var tasks = new List<Task>();

            for (int i = 0; i < connectionCount; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var ws = new ClientWebSocket();
                    ws.Options.SetRequestHeader("Authorization", _token);

                    await ws.ConnectAsync(new Uri(_url), CancellationToken.None);
                    Assert.Equal(WebSocketState.Open, ws.State);
                    byte[] buffer = MessageFactory.CreateMessageBuffer("testMessage");
              
                    await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                    await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                    var recv = new byte[4096];
                    var result = await ws.ReceiveAsync(recv, CancellationToken.None);

                    string reply = Encoding.UTF8.GetString(recv, 0, result.Count);
                    Assert.Equal("accomplished", reply);

                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}

