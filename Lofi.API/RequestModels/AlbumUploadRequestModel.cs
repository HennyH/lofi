using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

public class AlbumUploadRequestModel
{
    public AlbumUploadRequestModel()
    { }

    public string? AlbumTitle { get; set; } = null!;
    public string? AlbumDescription { get; set; } = null!;
    public string? ArtistName { get; set; } = null!;
    public string? ArtistWallet { get; set; } = null!;
    public IFormFile? AlbumCoverPhoto { get; set; } = null!;
    public IEnumerable<IFormFile> Songs { get; set; }
}