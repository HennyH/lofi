using Lofi.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Database
{
    public class LofiContext : DbContext
    {
        public LofiContext(DbContextOptions<LofiContext> options) : base(options)
        { }

        public virtual DbSet<UploadedFile> UploadedFiles { get; set; } = null!;
        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<Artist> Artists { get; set; } = null!;
        public virtual DbSet<Album> Albums { get; set; } = null!;
        public virtual DbSet<Track> Tracks { get; set; } = null!;
        public virtual DbSet<Tip> Tips { get; set; } = null!;
    }
}