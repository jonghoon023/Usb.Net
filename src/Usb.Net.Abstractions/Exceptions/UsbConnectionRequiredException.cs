using System;

namespace Usb.Net.Abstractions.Exceptions
{
    /// <summary>
    /// Exception thrown when a USB operation is attempted without an established connection.
    /// </summary>
    /// <remarks> This exception indicates that a method requiring an active USB connection was called before the connection was properly opened via <see cref="IUsbDevice.Open" />. </remarks>
    public sealed class UsbConnectionRequiredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbConnectionRequiredException" /> class.
        /// </summary>
        public UsbConnectionRequiredException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbConnectionRequiredException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UsbConnectionRequiredException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbConnectionRequiredException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception. </param>
        public UsbConnectionRequiredException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
