using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSearch
{
    public class BitcoinAddressGenerator
    {
        private const string Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string GenerateAddressFromPrivateKey(string privateKeyHex)
        {
            byte[] privateKey = HexToByteArray(privateKeyHex);
            byte[] publicKey = GetPublicKeyFromPrivateKey(privateKey);
            return GetAddressFromPublicKey(publicKey);
        }

        public static string PrivateKeyToWIF(string privateKeyHex)
        {
            byte[] privateKey = HexToByteArray(privateKeyHex);

            byte[] extendedKey = new byte[privateKey.Length + 1];
            extendedKey[0] = 0x80;
            Array.Copy(privateKey, 0, extendedKey, 1, privateKey.Length);

            byte[] firstSha = ComputeSHA256(extendedKey);
            byte[] secondSha = ComputeSHA256(firstSha);

            byte[] checksum = new byte[4];
            Array.Copy(secondSha, 0, checksum, 0, 4);

            byte[] keyWithChecksum = new byte[extendedKey.Length + 4];
            Array.Copy(extendedKey, 0, keyWithChecksum, 0, extendedKey.Length);
            Array.Copy(checksum, 0, keyWithChecksum, extendedKey.Length, 4);

            return Base58Encode(keyWithChecksum);
        }

        private static byte[] GetPublicKeyFromPrivateKey(byte[] privateKey)
        {
            using HMACSHA256 hmac = new(privateKey);
            byte[] publicKey = hmac.ComputeHash(privateKey);

            byte[] fullPublicKey = new byte[publicKey.Length + 1];
            fullPublicKey[0] = 0x04;
            Array.Copy(publicKey, 0, fullPublicKey, 1, publicKey.Length);

            return fullPublicKey;
        }

        private static string GetAddressFromPublicKey(byte[] publicKey)
        {
            byte[] sha256Hash = ComputeSHA256(publicKey);
            byte[] ripemd160Hash = ComputeRIPEMD160(sha256Hash);

            byte[] versionedHash = new byte[ripemd160Hash.Length + 1];
            versionedHash[0] = 0x00;
            Array.Copy(ripemd160Hash, 0, versionedHash, 1, ripemd160Hash.Length);

            byte[] firstSha = ComputeSHA256(versionedHash);
            byte[] secondSha = ComputeSHA256(firstSha);

            byte[] checksum = new byte[4];
            Array.Copy(secondSha, 0, checksum, 0, 4);

            byte[] addressBytes = new byte[versionedHash.Length + 4];
            Array.Copy(versionedHash, 0, addressBytes, 0, versionedHash.Length);
            Array.Copy(checksum, 0, addressBytes, versionedHash.Length, 4);

            return Base58Encode(addressBytes);
        }

        private static byte[] ComputeSHA256(byte[] data) => SHA256.HashData(data);

        private static byte[] ComputeRIPEMD160(byte[] data)
        {
            using HMACSHA256 hmac = new(data);
            byte[] hash = hmac.ComputeHash(data);

            byte[] result = new byte[20];
            Array.Copy(hash, result, 20);

            return result;
        }

        private static byte[] HexToByteArray(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex[2..];

            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length");

            byte[] result = new byte[hex.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return result;
        }

        private static string Base58Encode(byte[] data)
        {
            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            StringBuilder result = new();
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