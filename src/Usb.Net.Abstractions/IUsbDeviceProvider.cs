using System;
using System.Collections.Generic;
using System.Threading;

namespace Usb.Net.Abstractions
{
    /// <summary>
    /// Provides functionality to discover connected USB devices and create <see cref="IUsbDevice" /> instances for communication.
    /// </summary>
    /// <remarks> This interface abstracts platform-specific logic for device enumeration and communicator creation, allowing applications to interact with USB devices in a consistent and platform-independent way. </remarks>
    public interface IUsbDeviceProvider : IDisposable
    {
        /// <summary>
        /// Enumerates all currently connected USB devices.
        /// </summary>
        /// <param name="cancellationToken"> A token to monitor for cancellation requests. </param>
        /// <returns> A collection of <see cref="UsbDeviceInfo" /> objects representing connected USB devices. </returns>
        IEnumerable<UsbDeviceInfo> EnumerateDevices(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an <see cref="IUsbDevice" /> instance for the specified USB device.
        /// </summary>
        /// <param name="deviceInfo"> The information identifying the target USB device. </param>
        /// <returns> An <see cref="IUsbDevice" /> instance for communicating with the specified device. </returns>
        IUsbDevice CreateDevice(UsbDeviceInfo deviceInfo);
    }
}
