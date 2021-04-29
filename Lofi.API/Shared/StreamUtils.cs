using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Shared
{
    public static class StreamUtils
    {
        public static async Task<byte[]> ToArrayAsync(this Stream stream, int? capacity = null, CancellationToken cancellationToken = default)
        {
            using var ms = capacity == null ? new MemoryStream() : new MemoryStream(capacity.Value);
            await stream.CopyToAsync(ms, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return ms.ToArray();
        }
    }
}