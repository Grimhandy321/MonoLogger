using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AbstractTest
    {
        protected readonly IConfiguration _config;
        protected string _url;
        protected string _token;

        public AbstractTest()
        {
            var solutionDir = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "../../../../")
            );

            _config = new ConfigurationBuilder()
                .SetBasePath(solutionDir)
                .AddJsonFile("MonoLogger/appsettings.json", optional: false)
                .AddJsonFile("MonoLogger/appsettings.Development.json", optional: true)
                .Build();

            _url = _config["UnitTest:authTestToken"] ?? "ws://localhost:5151/ws";
            _token = _config["UnitTest:testUrl"] ?? "testToken";
        }

    }
}
