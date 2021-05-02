using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Database.Entities
{
    [Keyless]
    public class ArtistTipPayout
    {
        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }
        public int TipId { get; set; }
        public Tip? Tip { get; set; }
        public int PayoutId { get; set; }
        public TipPayout? Payout { get; set; }
    }
}