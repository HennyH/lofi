using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Album
    {
        protected Album()
        { }

        public Album(string title, string description, byte[] coverPhotoBytes, string coverPhotoMimeType, DateTime releaseDate, DateTime uploadDate, ICollection<Artist> artists, ICollection<Song> songs)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(nameof(title), "A song title cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(nameof(description), "A song description cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(coverPhotoMimeType))
            {
                throw new ArgumentException(nameof(coverPhotoMimeType), "A song coverPhotoMimeType cannot be empty");
            }

            // if (artists.Count == 0)
            // {
            //     throw new ArgumentException(nameof(artists), "An album must have at least one artist");
            // }

            // if (songs.Count == 0)
            // {
            //     throw new ArgumentException(nameof(songs), "An album must contain at least one song");
            // }

            this.Title = title;
            this.Description = description;
            this.CoverPhotoBytes = coverPhotoBytes;
            this.CoverPhotoMimeType = coverPhotoMimeType;
            this.ReleaseDate = releaseDate;
            this.UploadDate = uploadDate;
            this.Artists = artists;
            this.Songs = songs;
        }

        [Key]
        public int Id {get;set;}
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public byte[] CoverPhotoBytes { get; set; } = null!;
        public string CoverPhotoMimeType {get;set;} = null!;
        public DateTime ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; }
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}