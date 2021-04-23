using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Shared
{
    public static class StreamUtils
    {
        public static async Task<byte[]> ToArrayAsync(this Stream stream, int? capacity = null)
        {
            using var ms = capacity == null ? new MemoryStream() : new MemoryStream(capacity.Value);
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public static async Task<byte[]> ToArrayAsync(this IFormFile file, int? capacity = null)
        {
            using var stream = file.OpenReadStream();
            return await stream.ToArrayAsync(capacity);
        }
    }
}