using System;
using Usb.Net.Abstractions.Exceptions;

namespace Usb.Net.Abstractions
{
    /// <summary>
    /// Represents a USB communication device.
    /// </summary>
    public interface IUsbDevice : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the USB handle is currently open.
        /// </summary>
        /// <remarks> Returns <see langword="true" /> if the handle is open; otherwise, <see langword="false" />. </remarks>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the USB handle.
        /// </summary>
        /// <exception cref="UsbHandleException"> Thrown when an error occurs while attempting to create or open the USB device handle. </exception>
        void Open();

        /// <summary>
        /// Closes the USB handle.
        /// </summary>
        void Close();

        /// <summary>
        /// Writes data to the USB device.
        /// </summary>
        /// <param name="data"> The data to write. </param>
        /// <returns> <see langword="true" /> if the operation succeeds; otherwise, <see langword="false" />. </returns>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        bool Write(params byte[] data);

        /// <summary>
        /// Writes data to the USB device with retry attempts.
        /// </summary>
        /// <param name="retry"> The number of retry attempts. </param>
        /// <param name="data"> The data to write. </param>
        /// <returns> <see langword="true" /> if the operation succeeds; otherwise, <see langword="false" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="retry"/> is less than 1. </exception>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        bool Write(int retry, params byte[] data);

        /// <summary>
        /// Writes data to the USB device with a timeout.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the operation. </param>
        /// <param name="data"> The data to write. </param>
        /// <returns> <see langword="true" /> if the operation succeeds; otherwise, <see langword="false" />. </returns>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        bool Write(TimeSpan timeout, params byte[] data);

        /// <summary>
        /// Writes data to the USB device with retry attempts and a timeout.
        /// </summary>
        /// <param name="retry"> The number of retry attempts. </param>
        /// <param name="timeout"> The maximum time to wait for the operation. </param>
        /// <param name="data"> The data to write. </param>
        /// <returns> <see langword="true" /> if the operation succeeds; otherwise, <see langword="false" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="retry" /> is less than 1. </exception>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        bool Write(int retry, TimeSpan timeout, params byte[] data);

        /// <summary>
        /// Reads data from the USB device.
        /// </summary>
        /// <returns> The data read as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> Read();

        /// <summary>
        /// Reads data from the USB device with retry attempts.
        /// </summary>
        /// <param name="retry"> The number of retry attempts. </param>
        /// <returns> The data read as <see cref="ReadOnlyMemory{T}"/>. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="retry"/> is less than 1. </exception>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> Read(int retry);

        /// <summary>
        /// Reads data from the USB device with a timeout.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the operation. </param>
        /// <returns> The data read as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> Read(TimeSpan timeout);

        /// <summary>
        /// Reads data from the USB device with retry attempts and a timeout.
        /// </summary>
        /// <param name="retry"> The number of retry attempts. </param>
        /// <param name="timeout"> The maximum time to wait for the operation. </param>
        /// <returns> The data read as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="retry" /> is less than 1. </exception>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> Read(int retry, TimeSpan timeout);

        /// <summary>
        /// Writes data and then reads the response from the USB device.
        /// </summary>
        /// <param name="data"> The data to write. </param>
        /// <returns> The response as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> WriteThenRead(params byte[] data);

        /// <summary>
        /// Writes data and then reads the response from the USB device with retry attempts.
        /// </summary>
        /// <param name="retry"> The number of retry attempts. </param>
        /// <param name="data"> The data to write. </param>
        /// <returns> The response as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="retry"/> is less than 1. </exception>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> WriteThenRead(int retry, params byte[] data);

        /// <summary>
        /// Writes data and then reads the response from the USB device with a timeout.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the operation. </param>
        /// <param name="data"> The data to write. </param>
        /// <returns> The response as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> WriteThenRead(TimeSpan timeout, params byte[] data);

        /// <summary>
        /// Writes data and then reads the response from the USB device with retry attempts and a timeout.
        /// </summary>
        /// <param name="retry"> The number of retry attempts.</param>
        /// <param name="timeout"> The maximum time to wait for the operation. </param>
        /// <param name="data"> The data to write.</param>
        /// <returns> The response as <see cref="ReadOnlyMemory{T}" />. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="retry"/> is less than 1. </exception>
        /// <exception cref="UsbConnectionRequiredException"> Thrown when the USB connection has not been established. </exception>
        ReadOnlyMemory<byte> WriteThenRead(int retry, TimeSpan timeout, params byte[] data);
    }
}
