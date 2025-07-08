using System;

namespace Usb.Net.Internals.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="ReadOnlySpan{T}" /> of <see langword="byte" />.
    /// </summary>
    internal static class ReadOnlySpanExtensions
    {
        private const string Hyphen = "-";
        private const string Whitespace = " ";

        /// <summary>
        /// Converts the contents of the byte span to a hyphen-separated hexadecimal string.
        /// </summary>
        /// <param name="span"> The <see langword="byte" /> span to convert. </param>
        /// <returns> A string representing the hexadecimal values of the span. </returns>
        public static string ToHexString(this ReadOnlySpan<byte> span)
        {
            return BitConverter.ToString(span.ToArray()).Replace(Hyphen, Whitespace);
        }
    }
}
