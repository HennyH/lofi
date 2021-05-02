using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Lofi.API.Tests
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Lofi.API.Startup>
    {
        public ITestOutputHelper? OutputHelper { get; set; }
        public IConfiguration? Configuration { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.Test.json")
                    .Build();
                config.AddConfiguration(Configuration);
            });

            builder.ConfigureServices(services =>
            {
                if (OutputHelper != null)
                {
                    services.AddLogging(logging => logging.AddXunit(OutputHelper));
                }
            });

            builder.ConfigureTestServices(services =>
            {
                // 
            });
        }
    }
}