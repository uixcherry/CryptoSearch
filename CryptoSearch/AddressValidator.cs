namespace CryptoSearch
{
    public class AddressValidator
    {
        private HashSet<string> _existingAddresses = [];

        public void LoadAddressDatabase()
        {
            LoadDefaultAddresses();
        }

        public int LoadAddressDatabaseFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            try
            {
                string[] addresses = File.ReadAllLines(filePath);
                _existingAddresses = [.. addresses.Where(a => !string.IsNullOrWhiteSpace(a))];
                return _existingAddresses.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading address database: {ex.Message}", ex);
            }
        }

        public bool IsAddressExists(string address)
        {
            return _existingAddresses.Contains(address);
        }

        private void LoadDefaultAddresses()
        {
            string[] demoAddresses =
            [
                "1ADtEbG5UAnrqSTz8CssEPXFLYTM6mfCiA",
                "1CSPhHszH4S4Hhrrx8ZdmJWC3GLTpuhsur",
                "1LagHJk2FyCV2VzrNHVqg3gYG4TSYwDV4m",
                "179NZfVhPXssS42hs2o9ANABMcReQiovym",
                "1C77qaBvJ2mGAb9QmcuBHrD6sbYy9V8Rc7",
                "1Mv7xTXisEUQZ4aeQmDmmYKqLVw3PoNGA1",
                "1HxpsY48MpH4kvi2H23vDwdaBSa2paSipX",
                "137DbqLHaL32CLg91FYStaGi99y3H5xHNS",
                "1LvbRi7aCjENKCzDMTsaoZ8sKsg1B9CqLQ",
                "132vDcKDe96wL2GLLznueKnGVP4RnAX5xH",
                "134hoUfpzKWFqLMgrELRnERQemU3FAiY3v",
                "1LNLqCerQQ6j9iKs39VP6vNJHSpzNrFKJj",
                "182WMwUZYz4vksjnhzU6GpCtKW3g3HKwhb",
                "1P1Dur122Pzey73fFyUgfSbcxDdedSSm4z",
                "1EoXPE6MzT4EnHvk2Ldj64M2ks2EAcZyH4",
                "1Pt8N7xSic1oEKE8pPDEJ96KcnVFU69HRp",
                "1GgD7fHa8ZRczoMQagVzYc73ioTvoe3sZu",
                "1GTy3GAexCP74XVouxc7ZUP9s2qheQVDDz",
                "18XrReT5ChW8qgXecNgKTU5T6MrMMLnV8H",
                "1Cyr9LQ7F3PZNjiJCcafLLh6F1RDireoCn",
                "1LWWyaRPtU7PHHEN67Zh8qhmZyAoaRubcw",
                "15CHsSNH3SmEfoc2mau11z7inZiUZvRfyr",
                "18CW9Zv8rC8UcLSMHJGZSJ2uTHcqBEoa7D",
                "1EtppGCHU29KoJAwwU5sLdmeMim7GzBb5z",
                "1FB8cZijTpRQp3HX8AEkNuQJBqApqfTcX7",
                "19WcSz4FxGH7CRqChvXaFW42UHnbQBicky",
                "1EYQRFJ1z1WzhPs17sZPzYnWxoYXUs8cKj",
                "1EWCz4DrTShf9vFThywg2WfDyrdV4Wy2SN",
                "1Q4rfQLQF9yKN2Ay2NNBwLNPSy4sDbwNnd",
                "1Px5Kp4aZgPmVxJV9sWgLj6dEWcFMec9Fo",
                "1GDWJm5dPj6JTxF68WEVhicAS4gS3pvjo7",
                "1KmobiUwSFcKyDX2z1Ss2TVL5yLq2BnxrS",
                "1AS5u8iJsUFjQ3KmjkF5saBrHMLWdQMSpY",
                "13zGBax475zUoLFatjye7Rp48vPrtiaeqs",
                "1BoH6QH9n2pftQYRU5kganogxaNtTCo5cJ",
                "1LtUqEjvsTh4B5GtDmBV8upuRD6QcRTeCx",
                "1MsUtV4HwbEX6pnPa1cAfzJTn7oNufW5p4",
                "1KWj99Jwd9LGGC2Y1c9c4cmvWvYTQrLFVc",
                "1PnVcifB9EBwbaAauprqcucPgfUnrRLtWg",
                "19vxtDbLMNasSpbAEZd7va5Qge6d2zYWbp"
            ];

            _existingAddresses = [.. demoAddresses];
        }
    }
}