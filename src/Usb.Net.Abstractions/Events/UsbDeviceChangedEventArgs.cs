using System;

namespace Usb.Net.Abstractions.Events
{
    /// <summary>
    /// Represents the event data for USB device state changes, such as when a device is attached or detached.
    /// </summary>
    public sealed class UsbDeviceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the information of the USB device involved in the state change event.
        /// </summary>
        public UsbDeviceInfo DeviceInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbDeviceChangedEventArgs" /> class.
        /// </summary>
        /// <param name="deviceInfo"> An object containing details about the USB device that triggered the event. </param>
        public UsbDeviceChangedEventArgs(UsbDeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }
    }
}
