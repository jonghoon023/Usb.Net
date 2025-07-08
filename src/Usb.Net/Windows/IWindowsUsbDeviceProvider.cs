using System;
using System.Collections.Generic;
using System.Threading;
using Usb.Net.Abstractions;

namespace Usb.Net.Windows
{
    /// <summary>
    /// Provides a Windows-specific implementation of <see cref="IUsbDeviceProvider" /> for enumerating and accessing USB devices.
    /// </summary>
    public interface IWindowsUsbDeviceProvider : IUsbDeviceProvider
    {
        /// <summary>
        /// Enumerates all currently connected USB devices that match the specified device interface class.
        /// </summary>
        /// <param name="deviceInterfaceClass">
        /// The GUID representing the device interface class used to filter USB devices. <br />
        /// This corresponds to the device setup class or interface class GUID in Windows.
        /// </param>
        /// <param name="cancellationToken"> A token to monitor for cancellation requests. </param>
        /// <returns> A collection of <see cref="WindowsUsbDeviceInfo" /> objects representing the connected USB devices that match the specified interface class. </returns>
        IEnumerable<WindowsUsbDeviceInfo> EnumerateDevices(Guid deviceInterfaceClass, CancellationToken cancellationToken = default);
    }
}
