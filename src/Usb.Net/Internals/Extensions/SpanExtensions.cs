using System;

namespace Usb.Net.Internals.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Span{T}" />.
    /// </summary>
    internal static class SpanExtensions
    {
        /// <summary>
        /// Trims all leading and trailing occurrences of the specified element from the span.
        /// </summary>
        /// <typeparam name="T"> The type of elements in the span. </typeparam>
        /// <param name="span"> The source span to trim. </param>
        /// <param name="trimElement"> The element to remove from both the beginning and end of the span. </param>
        /// <returns> A trimmed span with leading and trailing matching elements removed. </returns>
        public static Span<T> Trim<T>(this Span<T> span, T trimElement) where T : IEquatable<T>
        {
            int start = 0;
            int end = span.Length - 1;

            while (start <= end && span[start].Equals(trimElement))
            {
                start++;
            }

            while (end >= start && span[end].Equals(trimElement))
            {
                end--;
            }

            int length = end - start + 1;
            return span.Slice(start, length);
        }
    }
}
