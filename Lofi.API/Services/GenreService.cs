using Lofi.Database;

namespace Lofi.API.Services
{
    public class GenreService
    {
        private readonly LofiContext _lofiContext;

        public GenreService(LofiContext lofiContext)
        {
            this._lofiContext = lofiContext;
        }
    }
}