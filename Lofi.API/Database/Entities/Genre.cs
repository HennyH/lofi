using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Genre
    {
        [Key]
        public int Id {get;set;}
        [Required]
        public string? Name {get;set;}
        public string? Desription {get;set;}
        public Genre? ParentGenre {get;set;}
        public int? ParentGenreId {get;set;}
    }
}