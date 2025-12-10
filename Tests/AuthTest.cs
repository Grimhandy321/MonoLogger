using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AuthTest
    {

        private readonly IConfiguration _config;

        public AuthTest()
        {
            _config = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)
           .AddJsonFile("../MonoLogger/appsettings.json", optional: false, reloadOnChange: false)
           .AddJsonFile("../MonoLogger/appsettings.Development.json", optional: true)
           .Build();
        }

        private readonly string _url = "ws://localhost:5151/ws";
        [Fact]
        public async Task WebSocket_ShouldRefuseConnection_WhenTokenInvalid()
        {
            using var ws = new ClientWebSocket();

            ws.Options.SetRequestHeader("Authorization", "totallyWrongToken");

            var exception = await Assert.ThrowsAsync<WebSocketException>(async () =>
            {
                await ws.ConnectAsync(new Uri(_url), CancellationToken.None);
            });

            Assert.Contains("401", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task WebSocket_ShouldRefuseConnection_WhenTokenMissing()
        {
            using var ws = new ClientWebSocket();

            var exception = await Assert.ThrowsAsync<WebSocketException>(async () =>
            {
                await ws.ConnectAsync(new Uri(_url), CancellationToken.None);
            });

            Assert.Contains("401", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
