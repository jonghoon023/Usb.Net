using System;
using Usb.Net.Abstractions;
using Usb.Net.Abstractions.Exceptions;

namespace Usb.Net.Windows
{
    /// <summary>
    /// Provides Windows-specific functionality for monitoring USB device plug-and-play (PnP) events.
    /// </summary>
    public interface IWindowsUsbMonitor : IUsbMonitor
    {
        /// <summary>
        /// Starts monitoring the system for USB device plug-and-play (PnP) events filtered by the specified device interface class GUID.
        /// </summary>
        /// <param name="deviceInterfaceClass"> The device interface class GUID used to filter USB devices for monitoring. </param>
        /// <remarks> To stop monitoring, call <see cref="IUsbMonitor.StopMonitoring" />. </remarks>
        /// <exception cref="UsbMonitoringException"> Thrown when an error occurs while attempting to start USB device monitoring. </exception>
        /// <exception cref="ObjectDisposedException"> Thrown when this instance has already been disposed and monitoring cannot be started. </exception>
        void StartMonitoring(Guid deviceInterfaceClass);
    }
}
