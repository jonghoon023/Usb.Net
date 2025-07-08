using System;
using System.Collections.Generic;
using System.Globalization;

namespace Usb.Net.Avalonia.Models
{
    /// <summary>
    /// Provides utility methods for converting between hexadecimal strings and byte arrays.
    /// </summary>
    public static class HexConverter
    {
        private const char Hyphen = '-';
        private const char WhiteSpace = ' ';

        /// <summary>
        /// Converts a whitespace-delimited hexadecimal string into a byte array.
        /// </summary>
        /// <param name="hexString"> A string containing hexadecimal byte values separated by spaces (e.g., <c> "1A 2B 3C" </c>). </param>
        /// <returns>
        /// A byte array parsed from the hexadecimal string. <br />
        /// If the input is <c>null</c> or empty, an empty array is returned. <br />
        /// Invalid byte values are ignored.
        /// </returns>
        public static byte[] ToBytes(string? hexString)
        {
            string[] hexValuesSplit = hexString?.Split(WhiteSpace) ?? Array.Empty<string>();
            List<byte> bytes = new List<byte>();

            foreach (string hexValue in hexValuesSplit)
            {
                if (byte.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte byteValue))
                {
                    bytes.Add(byteValue);
                }
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Converts a byte array into a compact hexadecimal string with no delimiters.
        /// </summary>
        /// <param name="bytes"> The byte array to convert. </param>
        /// <returns> A string containing hexadecimal characters representing the input bytes (e.g., "1A2B3C"). </returns>
        public static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace(Hyphen, WhiteSpace);
        }
    }
}
