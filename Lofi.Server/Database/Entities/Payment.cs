using System;
using System.ComponentModel.DataAnnotations;

namespace Lofi.Server.Database.Entities
{
    public class Tip
    {
        protected Tip()
        {
            this.TransactionHash = null!;
            this.Song = null!;
            this.Payee = null!;
        }

        public Tip(string transactionHash, Song song, Artist payee, DateTime? payedAt = null)
        {
            this.TransactionHash = transactionHash;
            this.Song = song;
            this.Payee = payee;
            this.PayedAt = payedAt ?? DateTime.Now;
        }

        [Key]
        public string TransactionHash { get; set; }
        public DateTime PayedAt { get; set; }
        public virtual Song Song { get; set; }
        public virtual Artist Payee { get; set; }
    }
}