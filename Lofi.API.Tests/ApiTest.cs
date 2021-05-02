using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
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

        public ApiTestFixture(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
        {
            this._factory = factory;
            this._factory.OutputHelper = outputHelper;
            this._client = factory.CreateClient();
        }
    }
}
