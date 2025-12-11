using System.Net.WebSockets;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class AuthTest : AbstractTest
    {
        [Fact]
        public async Task WebSocket_ShouldRefuseConnection_WhenTokenInvalid()
        {
            using var ws = new ClientWebSocket();

            ws.Options.SetRequestHeader("Authorization", "totallyWrongToken");

            var exception = await Assert.ThrowsAsync<WebSocketException>(() =>
                ws.ConnectAsync(new Uri(_url), CancellationToken.None)
            );

            Assert.Contains("401", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task WebSocket_ShouldRefuseConnection_WhenTokenMissing()
        {
            using var ws = new ClientWebSocket();

            var exception = await Assert.ThrowsAsync<WebSocketException>(() =>
                ws.ConnectAsync(new Uri(_url), CancellationToken.None)
            );

            Assert.Contains("401", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
