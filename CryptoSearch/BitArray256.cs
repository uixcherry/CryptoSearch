using System.Security.Cryptography;
using System.Text;

namespace CryptoSearch
{
    public class BitArray256
    {
        private readonly byte[] _cells = new byte[32];
        private readonly bool[] _fixedCells = new bool[32];
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public BitArray256()
        {
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < 32; i++)
            {
                _cells[i] = 0;
                _fixedCells[i] = false;
            }
        }

        public byte GetCellValue(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(cellIndex));

            return _cells[cellIndex];
        }

        public void SetCellValue(int cellIndex, byte value)
        {
            if (cellIndex < 0 || cellIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(cellIndex));

            _cells[cellIndex] = value;
        }

        public bool IsCellFixed(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(cellIndex));

            return _fixedCells[cellIndex];
        }

        public void ToggleCellFixed(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(cellIndex));

            _fixedCells[cellIndex] = !_fixedCells[cellIndex];
        }

        public void SetCellFixed(int cellIndex, bool isFixed)
        {
            if (cellIndex < 0 || cellIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(cellIndex));

            _fixedCells[cellIndex] = isFixed;
        }

        public void RandomizeNonFixedCells()
        {
            byte[] randomBytes = new byte[32];
            _rng.GetBytes(randomBytes);

            for (int i = 0; i < 32; i++)
            {
                if (!_fixedCells[i])
                {
                    _cells[i] = randomBytes[i];
                }
            }
        }

        public void RandomizeNonFixedCellsWithFilter()
        {
            byte[] randomBytes = new byte[32];
            _rng.GetBytes(randomBytes);

            for (int i = 0; i < 32; i++)
            {
                if (!_fixedCells[i])
                {
                    randomBytes[i] |= 0x11;
                    _cells[i] = randomBytes[i];
                }
            }
        }

        public void RandomizeNonFixedCellsWithProbabilisticFilter()
        {
            byte[] randomBytes = new byte[32];
            _rng.GetBytes(randomBytes);

            for (int i = 0; i < 32; i++)
            {
                if (!_fixedCells[i])
                {
                    if (randomBytes[i] % 10 < 8)
                    {
                        if (randomBytes[i] % 2 == 0)
                            randomBytes[i]++;
                    }
                    _cells[i] = randomBytes[i];
                }
            }
        }

        public byte[] ToByteArray()
        {
            byte[] result = new byte[32];
            Array.Copy(_cells, result, 32);
            return result;
        }

        public string ToHexString()
        {
            StringBuilder sb = new();
            foreach (byte b in _cells)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public void FromHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                throw new ArgumentNullException(nameof(hexString));

            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hexString = hexString[2..];

            if (hexString.Length != 64)
                throw new ArgumentException("Hex string must be 64 characters long", nameof(hexString));

            for (int i = 0; i < 32; i++)
            {
                _cells[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
        }

        public BitArray256 Clone()
        {
            BitArray256 clone = new();

            for (int i = 0; i < 32; i++)
            {
                clone._cells[i] = _cells[i];
                clone._fixedCells[i] = _fixedCells[i];
            }

            return clone;
        }
    }
}