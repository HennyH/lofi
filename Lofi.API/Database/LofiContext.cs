using System.Collections.Generic;
using Lofi.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Database
{
    public class LofiContext : DbContext
    {
        public LofiContext(DbContextOptions<LofiContext> options) : base(options)
        { }

        public virtual DbSet<Artist> Artists { get; set; } = null!;
        public virtual DbSet<Song> Songs { get; set; } = null!;
        public virtual DbSet<Tip> Tips { get; set; } = null!;
        public virtual DbSet<Album> Albums { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.Entity<Album>().HasMany(album => album.Artists).WithMany(artist => artist.Albums);
            // builder.Entity<Album>().HasMany(album => album.Songs).WithOne(song => song!.Album);
            // builder.Entity<Song>().HasMany<Artist>(song => song.Artists).WithMany(artist => artist.Songs);
        }
    }
}