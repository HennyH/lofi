using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Shared
{
    public static class FormFileUtils
    {
        public static async Task<byte[]> ToArrayAsync(this IFormFile file, int? capacity = null, CancellationToken cancellationToken = default)
        {
            using var stream = file.OpenReadStream();
            return await stream.ToArrayAsync(capacity, cancellationToken);
        }
    }
}