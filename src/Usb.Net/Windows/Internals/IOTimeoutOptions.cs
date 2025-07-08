using System;

namespace Usb.Net.Windows.Internals
{
    /// <summary>
    /// Represents a set of timeout and delay values used for I/O operations.
    /// </summary>
    internal readonly struct IOTimeoutOptions
    {
        private const int Zero = 0;

        /// <summary>
        /// Gets the default <see cref="IOTimeoutOptions" /> instance.
        /// </summary>
        /// <remarks> The default <see cref="Timeout" /> is 3 seconds, and the default <see cref="Delay" /> is 100 milliseconds. </remarks>
        public static IOTimeoutOptions Default => new(TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(100));

        /// <summary>
        /// Gets the default timeout duration.
        /// </summary>
        public static TimeSpan DefaultTimeout => Default.Timeout;

        /// <summary>
        /// Gets the default delay duration.
        /// </summary>
        public static TimeSpan DefaultDelay => Default.Delay;

        /// <summary>
        /// Initializes a new instance of the <see cref="IOTimeoutOptions" /> struct with the specified timeout and delay.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for an operation. </param>
        /// <param name="delay"> The delay interval between retries. </param>
        public IOTimeoutOptions(TimeSpan timeout, TimeSpan delay)
        {
            Timeout = timeout;
            Delay = delay;
        }

        /// <summary>
        /// Gets the timeout duration.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the delay duration between retries.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the total duration by summing <see cref="Timeout" /> and <see cref="Delay" />.
        /// </summary>
        public TimeSpan TotalDuration => Timeout + Delay;

        /// <summary>
        /// Creates a new <see cref="IOTimeoutOptions" /> instance from a total duration, subtracting the default delay to calculate the effective timeout.
        /// </summary>
        /// <param name="totalDuration"> The total duration for timeout and delay. </param>
        /// <returns> A new instance of <see cref="IOTimeoutOptions" />. </returns>
        public static IOTimeoutOptions FromTotalDuration(TimeSpan totalDuration)
        {
            double defaultDelayInMs = DefaultDelay.TotalMilliseconds;
            double timeoutInMs = Math.Max(totalDuration.TotalMilliseconds - defaultDelayInMs, Zero);

            return new IOTimeoutOptions(TimeSpan.FromMilliseconds(timeoutInMs), TimeSpan.FromMilliseconds(defaultDelayInMs));
        }

        /// <summary>
        /// Creates a new <see cref="IOTimeoutOptions" /> instance from a total duration in milliseconds.
        /// </summary>
        /// <param name="totalTimeInMilliseconds"> The total duration in milliseconds. </param>
        /// <returns> A new instance of <see cref="IOTimeoutOptions" />. </returns>
        public static IOTimeoutOptions FromTotalDuration(double totalTimeInMilliseconds)
        {
            TimeSpan totalTime = TimeSpan.FromMilliseconds(totalTimeInMilliseconds);
            return FromTotalDuration(totalTime);
        }
    }
}
