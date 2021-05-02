using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Lofi.API.Tests.Utilities.HttpContentUtilities
{
    public static class HttpContentUtilities
    {
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
    }
}