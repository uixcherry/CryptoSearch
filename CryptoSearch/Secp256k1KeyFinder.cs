using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSearch
{
    public class Secp256k1KeyFinder
    {
        private HashSet<byte[]> _compressedPublicKeys = new HashSet<byte[]>(new ByteArrayComparer());
        private Dictionary<string, byte> _networkTypes = new Dictionary<string, byte>();
        private int _matchesFound = 0;
        private List<KeyMatch> _matches = new List<KeyMatch>();

        public Secp256k1KeyFinder() { }

        public int LoadPublicKeyDatabase(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("База публичных ключей не найдена", filePath);

            _compressedPublicKeys.Clear();

            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                int keyCount = data.Length / 34;

                for (int i = 0; i < keyCount; i++)
                {
                    byte[] pubKey = new byte[33];
                    Array.Copy(data, i * 34, pubKey, 0, 33);

                    byte networkType = data[i * 34 + 33];

                    _compressedPublicKeys.Add(pubKey);

                    string keyHash = BitConverter.ToString(pubKey).Replace("-", "");
                    _networkTypes[keyHash] = networkType;
                }

                return _compressedPublicKeys.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки базы публичных ключей: {ex.Message}", ex);
            }
        }

        public async Task<int> SearchMatchingKeysAsync(BitArray256 privateKeyTemplate, int attempts, IProgress<string> progress = null)
        {
            _matchesFound = 0;
            _matches.Clear();

            return await Task.Run(() =>
            {
                for (int i = 0; i < attempts; i++)
                {
                    BitArray256 testKey = privateKeyTemplate.Clone();
                    testKey.RandomizeNonFixedCellsWithFilter();

                    byte[] privateKeyBytes = testKey.ToByteArray();
                    byte[] compressedPublicKey = GenerateCompressedPublicKeyFromPrivate(privateKeyBytes);

                    if (_compressedPublicKeys.Contains(compressedPublicKey))
                    {
                        _matchesFound++;

                        string keyHash = BitConverter.ToString(compressedPublicKey).Replace("-", "");
                        byte networkType = _networkTypes.ContainsKey(keyHash) ? _networkTypes[keyHash] : (byte)0;

                        KeyMatch match = new KeyMatch
                        {
                            PrivateKey = testKey.ToHexString(),
                            PublicKeyCompressed = BitConverter.ToString(compressedPublicKey).Replace("-", ""),
                            NetworkType = networkType,
                            Address = DeriveAddressFromPublicKey(compressedPublicKey, networkType)
                        };

                        _matches.Add(match);

                        progress?.Report($"[{DateTime.Now:HH:mm:ss}] Найдено совпадение: {match.Address} (сеть: {GetNetworkName(networkType)})");
                    }

                    if (i % 1000 == 0)
                    {
                        progress?.Report($"Прогресс: {i}/{attempts} ({i * 100.0 / attempts:F2}%) Найдено: {_matchesFound}");
                    }
                }

                return _matchesFound;
            });
        }

        public List<KeyMatch> GetMatches() => _matches;

        private byte[] GenerateCompressedPublicKeyFromPrivate(byte[] privateKey)
        {
            using HMACSHA256 hmac = new(privateKey);
            byte[] hash = hmac.ComputeHash(privateKey);

            byte[] compressedPublicKey = new byte[33];
            compressedPublicKey[0] = (byte)(hash[31] % 2 == 0 ? 0x02 : 0x03);
            Array.Copy(hash, 0, compressedPublicKey, 1, 32);

            return compressedPublicKey;
        }

        private string DeriveAddressFromPublicKey(byte[] compressedPublicKey, byte networkType)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] sha256Hash = sha256.ComputeHash(compressedPublicKey);

            byte[] ripemd160Hash = new byte[20];
            Array.Copy(sha256Hash, ripemd160Hash, 20);

            byte[] versionedHash = new byte[21];
            versionedHash[0] = GetNetworkVersionByte(networkType);
            Array.Copy(ripemd160Hash, 0, versionedHash, 1, 20);

            byte[] checksum = sha256.ComputeHash(sha256.ComputeHash(versionedHash));

            byte[] addressBytes = new byte[25];
            Array.Copy(versionedHash, 0, addressBytes, 0, 21);
            Array.Copy(checksum, 0, addressBytes, 21, 4);

            return Base58Encode(addressBytes);
        }

        private byte GetNetworkVersionByte(byte networkType)
        {
            return networkType switch
            {
                0 => 0x00, // Bitcoin mainnet
                1 => 0x6f, // Bitcoin testnet
                2 => 0x3c, // Ethereum
                3 => 0x38, // Binance
                _ => 0x00  // По умолчанию - Bitcoin
            };
        }

        private string GetNetworkName(byte networkType)
        {
            return networkType switch
            {
                0 => "Bitcoin",
                1 => "Bitcoin Testnet",
                2 => "Ethereum",
                3 => "Binance",
                4 => "Arbitrum",
                5 => "Ethereum Classic",
                _ => "Неизвестная"
            };
        }

        private string Base58Encode(byte[] data)
        {
            const string Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            StringBuilder result = new StringBuilder();
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result.Insert(0, Base58Chars[remainder]);
            }

            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result.Insert(0, Base58Chars[0]);
            }

            return result.ToString();
        }
    }
}