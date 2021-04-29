using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Lofi.API.Shared;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Database.Entities
{
    public class UploadedFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid? UniqueId { get; set; }
        [Required]
        public string? FileName { get; set; }
        public string? Extension { get; set; }
        [Required]
        public string? MimeType { get; set; }
        [Required]
        public byte[]? Bytes { get; set; }
        [Required]
        public string? Hash {get;set;}
        [Required]
        public bool Deleted { get; set; } = false;       

        public static async Task<UploadedFile> FromFormFile(IFormFile formFile)
        {
            var bytes = await formFile.ToArrayAsync();
            var hash = await HashUtils.Sha1HexDigestAsync(bytes);
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