namespace CryptoSearch
{
    public class KeyMatch
    {
        public string PrivateKey { get; set; }
        public string PublicKeyCompressed { get; set; }
        public byte NetworkType { get; set; }
        public string Address { get; set; }
    }
}