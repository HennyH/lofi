using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Database.Entities
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? WalletAddress { get; set; }
        public UploadedFile? ProfilePicture { get; set; }
    }
}