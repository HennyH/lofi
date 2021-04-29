using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Lofi.API.Database;
using Lofi.API.Database.Entities;

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