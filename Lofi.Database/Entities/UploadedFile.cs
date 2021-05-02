using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

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
    }
}