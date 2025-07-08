using System;
using Usb.Net.Abstractions.Events;
using Usb.Net.Abstractions.Exceptions;

namespace Usb.Net.Abstractions
{
    /// <summary>
    /// Provides functionality to monitor USB device plug-and-play (PnP) events.
    /// </summary>
    public interface IUsbMonitor : IDisposable
    {
        /// <summary>
        /// Occurs when a USB device is attached to the system.
        /// </summary>
        event EventHandler<UsbDeviceChangedEventArgs> DeviceAttached;

        /// <summary>
        /// Occurs when a USB device is detached from the system.
        /// </summary>
        event EventHandler<UsbDeviceChangedEventArgs> DeviceDetached;

        /// <summary>
        /// Gets a value indicating whether USB device monitoring is currently active.
        /// </summary>
        bool IsMonitoring { get; }

        /// <summary>
        /// Starts monitoring the system for USB device plug-and-play (PnP) events.
        /// </summary>
        /// <remarks> To stop monitoring, call <see cref="StopMonitoring" />. </remarks>
        /// <exception cref="UsbMonitoringException"> Thrown when an error occurs while attempting to start USB device monitoring. </exception>
        /// <exception cref="ObjectDisposedException"> Thrown when this instance has already been disposed and monitoring cannot be started. </exception>
        void StartMonitoring();

        /// <summary>
        /// Stops monitoring for USB device plug-and-play (PnP) events.
        /// </summary>
        void StopMonitoring();
    }
}
