using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lofi.API.Tests.Utilities.HttpContentUtilities
{
    public static class HttpContentUtilities
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        public static StreamContent FileUploadContent(string path, string mimeType, string fieldName, string fileName)
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

        public static StringContent JsonContent<T>(T data) =>
            new StringContent(JsonSerializer.Serialize<T>(data, _jsonOptions), Encoding.UTF8, "application/json");
    }
}