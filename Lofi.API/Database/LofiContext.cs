using Lofi.API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

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
        public virtual DbSet<TipPayout> TipPayouts { get; set; } = null!;
        public virtual DbSet<TipPayment> TipPayments { get; set; } = null!;
        public virtual DbSet<ArtistTipPayout> ArtistTipPayouts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options
            .UseNpgsql()
            .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
            .UseSnakeCaseNamingConvention();
    }
}