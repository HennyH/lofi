using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Lofi.API.Tests.Utilities
{
    public static class HttpResponseMessageUtilities
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        public static async Task<T?> DeserializeAsJson<T>(this HttpResponseMessage response) =>
            await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonOptions);

        public static async Task<T> ParseContentAsync<T>(this HttpResponseMessage response, Func<string, T> parser)
        {
            var text = await response.Content.ReadAsStringAsync();
            T? result = default;
            var parsingException = Record.Exception(() => result = parser(text));
            parsingException.Should().BeNull($"parsing the response content \"{text}\" as a {typeof(T)} should not have failed, but it did with the following exception: {parsingException}");
            return result!;
        }
    }
}