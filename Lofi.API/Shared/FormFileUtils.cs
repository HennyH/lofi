using System;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
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

        public static async Task<UploadedFile> AsUploadedFile(this IFormFile formFile, CancellationToken cancellationToken = default)
        {
            var bytes = await formFile.ToArrayAsync(cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var hash = await HashUtils.Sha1HexDigestAsync(bytes, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return new UploadedFile
            {
                UniqueId = Guid.NewGuid(),
                FileName = formFile.FileName,
                Extension = MimeTypeUtils.GetExtension(formFile.ContentType),
                MimeType = formFile.ContentType,
                Bytes = bytes,
                Hash = hash
            };
        }
    }
}