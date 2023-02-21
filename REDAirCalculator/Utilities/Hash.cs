using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace REDAirCalculator.Controllers
{
    public static class Hash
    {
        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static string Crypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Base32.ToBase32String(outputBuffer);
        }

        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Base32.ToByteArray(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}

public static class Base32
{
    private const string Base32AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string ToBase32String(this byte[] input, bool addPadding = true)
    {
        if (input == null || input.Length == 0)
        {
            return string.Empty;
        }

        var bits = input.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Aggregate((a, b) => a + b).PadRight((int)(Math.Ceiling((input.Length * 8) / 5d) * 5), '0');
        var result = Enumerable.Range(0, bits.Length / 5).Select(i => Base32AllowedCharacters.Substring(Convert.ToInt32(bits.Substring(i * 5, 5), 2), 1)).Aggregate((a, b) => a + b);
        if (addPadding)
        {
            result = result.PadRight((int)(Math.Ceiling(result.Length / 8d) * 8), '=');
        }
        return result;
    }

    public static string EncodeAsBase32String(this string input, bool addPadding = true)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var bytes = Encoding.UTF8.GetBytes(input);
        var result = bytes.ToBase32String(addPadding);
        return result;
    }

    public static string DecodeFromBase32String(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var bytes = input.ToByteArray();
        var result = Encoding.UTF8.GetString(bytes);
        return result;
    }

    /// <summary>
    /// Converts a Base32 string into the corresponding byte array, using 5 bits per character.
    /// </summary>
    /// <param name="input">The Base32 String</param>
    /// <returns>A byte array containing the properly encoded bytes.</returns>
    public static byte[] ToByteArray(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new byte[0];
        }

        var bits = input.TrimEnd('=').ToUpper().ToCharArray().Select(c => Convert.ToString(Base32AllowedCharacters.IndexOf(c), 2).PadLeft(5, '0')).Aggregate((a, b) => a + b);
        var result = Enumerable.Range(0, bits.Length / 8).Select(i => Convert.ToByte(bits.Substring(i * 8, 8), 2)).ToArray();
        return result;
    }
}
