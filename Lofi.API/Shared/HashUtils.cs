using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lofi.API.Shared
{
    public static class HashUtils
    {
        public static async Task<string> Sha1HexDigestAsync(byte[] bytes)
        {
            using var sha1 = new SHA1Managed();
            using var byteStream = new MemoryStream(bytes);
            var hash = await sha1.ComputeHashAsync(byteStream);
            return Convert.ToHexString(hash);
        }
    }
}