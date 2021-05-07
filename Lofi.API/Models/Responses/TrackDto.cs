namespace Lofi.API.Models.Responses
{
    public class TrackDto
    {
        public int? TrackId { get; set; }
        public string? TrackName { get; set; }
        public string? TrackDescription { get; set; }
        public int? AlbumId { get; set; }
        public string? AlbumName { get; set; }
        public string? AlbumDescription { get; set; }
    }
}