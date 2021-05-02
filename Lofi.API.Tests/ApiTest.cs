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

        protected async Task<T?> DeserializeJsonAsync<T>(HttpResponseMessage response) => await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
        protected async Task<T> ParseResponseAsAsync<T>(HttpResponseMessage response, Func<string, T> parser)
        {
            var text = await response.Content.ReadAsStringAsync();
            T? result = default;
            var parsingException = Record.Exception(() => result = parser(text));
            parsingException.Should().BeNull($"Parsing the response content \"{text}\" as a {typeof(T)} failed with the following exception: {parsingException}");
            return result!;
        }

        public StreamContent FileUploadContent(string path, string mimeType, string fieldName, string fileName)
        {
            var file = new StreamContent(File.OpenRead(path));
            file.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            file.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = fieldName,
                FileName = fileName
            };
            return file;
        }
    }
}
