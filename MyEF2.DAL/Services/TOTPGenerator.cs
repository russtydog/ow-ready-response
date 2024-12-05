using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class TOTPGenerator
    {
        public static string GenerateTOTPSecret()
        {
            byte[] secretKeyBytes = new byte[16]; // 16 bytes for a 128-bit key
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secretKeyBytes);
            }

            // Convert the byte array to a Base32-encoded string
            string secretKeyBase32 = Base32Encoding.ToString(secretKeyBytes);

            return secretKeyBase32;
        }
    }

    public class Base32Encoding
    {
        private const string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string ToString(byte[] bytes)
        {
            int bits = 0;
            int buffer = 0;
            int bufferSize = 0;
            char[] result = new char[(int)Math.Ceiling(bytes.Length * 8 / 5.0)];
            int count = 0;

            foreach (byte b in bytes)
            {
                buffer = (buffer << 8) | b;
                bufferSize += 8;
                while (bufferSize >= 5)
                {
                    result[count++] = _base32Chars[(buffer >> (bufferSize - 5)) & 0x1F];
                    bufferSize -= 5;
                }
            }

            if (bufferSize > 0)
            {
                result[count++] = _base32Chars[(buffer << (5 - bufferSize)) & 0x1F];
            }

            return new string(result, 0, count);
        }
    }


}
