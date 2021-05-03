using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Lofi.API.Tests
{
    public abstract class ApiTestFixture : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly Checkpoint _checkpoint = new Checkpoint
        {
            SchemasToInclude = new[] { "lofi_test" },
            WithReseed = true
        };
        protected readonly HttpClient _client;
        protected readonly ApiWebApplicationFactory _factory;
        protected readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        protected readonly ITestOutputHelper _output;

        public ApiTestFixture(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
        {
            this._factory = factory;
            this._factory.OutputHelper = outputHelper;
            this._client = factory.CreateClient();
            this._output = outputHelper;
        }

        protected T GetRequiredService<T>()
            where T : class
        {
            return this._factory.Services.GetRequiredService<T>();
        }
    }
}
