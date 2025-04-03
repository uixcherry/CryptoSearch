namespace CryptoSearch
{
    public class KeyMatch
    {
        public string PrivateKey { get; set; } = string.Empty;
        public string PublicKeyCompressed { get; set; } = string.Empty;
        public byte NetworkType { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}