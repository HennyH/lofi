using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Song
    {
        protected Song()
        { }

        public Song(string hash,
                    string title,
                    string description,
                    string genre,
                    byte[] audioBytes,
                    string audioMimeType,
                    byte[] coverPhotoBytes,
                    string coverPhotoMimeType,
                    ICollection<Artist> artists,
                    DateTime? uploadedAt = null)
        {
            // if (artists.Count < 1)
            // {
            //     throw new ArgumentOutOfRangeException(nameof(artists), "A song requires at least one artist to be listed.");
            // }

            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new ArgumentException(nameof(hash), "A hash of the audio must be provided");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(nameof(title), "A song must have a title.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(nameof(description), "A song must have a description.");
            }

            if (string.IsNullOrWhiteSpace(genre))
            {
                throw new ArgumentException(nameof(genre), "A song must have a genre.");
            }

            this.Hash = hash;
            this.Title = title;
            this.Description = description;
            this.Genre = genre;
            this.UploadAt = uploadedAt ?? DateTime.Now;
            this.AudioBytes = audioBytes;
            this.AudioMimeType = audioMimeType;
            this.CoverPhotoBytes = coverPhotoBytes;
            this.CoverPhotoMimeType = coverPhotoMimeType;
            this.Artists = artists;
        }

        [Key]
        public int Id { get; set; }
        public string Hash { get; set; } = null!;
        public DateTime UploadAt { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverPhotoMimeType { get; set; } = null!;
        public byte[] CoverPhotoBytes { get; set; } = null!;
        public string AudioMimeType { get; set; } = null!;
        public byte[] AudioBytes { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public Album? Album { get; set; }
        public int? AlbumId { get; set; }
    }
}