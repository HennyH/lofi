namespace Lofi.Server.Database.Entities
{
    public class Artist
    {
        public Artist(string name, string walletAddress, string gpgPublicKey, string? reference = null)
        {
            this.Name = name;
            this.WalletAddress = walletAddress;
            this.GpgPublicKey = gpgPublicKey;
            this.Reference = reference;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Reference { get; set; }
        public string WalletAddress { get; set; }
        public string GpgPublicKey { get; set; }
    }
}