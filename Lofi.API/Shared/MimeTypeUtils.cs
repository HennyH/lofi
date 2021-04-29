using System.Collections.Generic;
using MimeTypes;

namespace Lofi.API.Shared
{
    public static class MimeTypeUtils
    {
        public static readonly ICollection<string> MUSIC_MIME_TYPES = new HashSet<string>
        {
            "audio/mp3",
            "audio/flac",
            "audio/wav"
        };

        public static readonly ICollection<string> PHOTO_MIME_TYPES = new HashSet<string>
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/bmp",
            "image/webp"
        };

        public static bool IsMusicMimeType(string mimeType) => MUSIC_MIME_TYPES.Contains(mimeType);
        public static bool IsPhotoMimeType(string mimeType) => PHOTO_MIME_TYPES.Contains(mimeType);
        public static string? GetExtension(string mimeType)
        {
            var extension = MimeTypeMap.GetExtension(mimeType, throwErrorIfNotFound: false);
            return string.IsNullOrWhiteSpace(extension) ? null : extension;
        }
    }
}