using System;

namespace Usb.Net.Abstractions.Exceptions
{
    /// <summary>
    /// Exception thrown when an error occurs while creating or managing a USB device handle.
    /// </summary>
    /// <remarks>
    /// This exception typically occurs when the underlying system fails to create a valid USB handle, <br />
    /// for example, due to an invalid device path, insufficient permissions, or low-level system errors.
    /// </remarks>
    public sealed class UsbHandleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbHandleException" /> class.
        /// </summary>
        public UsbHandleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbHandleException" /> class with a specified error message.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public UsbHandleException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbHandleException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception. </param>
        public UsbHandleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
