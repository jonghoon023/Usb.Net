using System;

namespace Usb.Net.Abstractions.Exceptions
{
    /// <summary>
    /// Represents errors that occur during USB device monitoring operations.
    /// </summary>
    public class UsbMonitoringException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbMonitoringException" /> class.
        /// </summary>
        public UsbMonitoringException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbMonitoringException" /> class with a specified error message that describes the error.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public UsbMonitoringException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbMonitoringException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        /// <param name="innerException"> The exception that caused the current exception. </param>
        public UsbMonitoringException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
