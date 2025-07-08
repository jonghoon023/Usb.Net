using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Usb.Net.Abstractions;

namespace Usb.Net.Windows
{
    /// <summary>
    /// Represents USB device information specific to the Windows platform.
    /// </summary>
    public sealed class WindowsUsbDeviceInfo : UsbDeviceInfo
    {
        private static readonly Regex _getIdFromDevicePathRegex = new("\\w*_\\w*");
        private static readonly Regex _validSymbolicLinkRegex = new(@"^(##\?#|\\\\\?\\|\?\\)USB#VID_[0-9A-Fa-f]{4}&PID_[0-9A-Fa-f]{4}#.*#{([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})}", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsUsbDeviceInfo" /> class using numeric identifiers.
        /// </summary>
        /// <param name="productId"> The product ID that identifies the device model. </param>
        /// <param name="vendorId"> The vendor ID assigned to the device manufacturer. </param>
        /// <param name="path"> The platform-specific interface path used to access the device. </param>
        /// <param name="serialNumber"> The unique serial number of the USB device. </param>
        public WindowsUsbDeviceInfo(uint productId, uint vendorId, string path, string serialNumber)
            : base(productId, vendorId, path, serialNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsUsbDeviceInfo" /> class using hexadecimal string identifiers.
        /// </summary>
        /// <param name="productId"> The product ID in hexadecimal string format (e.g., <c> "3A21" </c>). </param>
        /// <param name="vendorId"> The vendor ID in hexadecimal string format (e.g., <c> "1C8A" </c>). </param>
        /// <param name="path"> The platform-specific interface path used to access the device. </param>
        /// <param name="serialNumber"> The unique serial number of the USB device. </param>
        public WindowsUsbDeviceInfo(string productId, string vendorId, string path, string serialNumber)
            : base(productId, vendorId, path, serialNumber)
        {
        }

        /// <summary>
        /// Creates a <see cref="WindowsUsbDeviceInfo" /> instance from a Windows USB symbolic link string.
        /// </summary>
        /// <param name="symbolicLink"> The Windows USB symbolic link string. </param>
        /// <returns> A <see cref="WindowsUsbDeviceInfo" /> instance parsed from the symbolic link. </returns>
        /// <exception cref="ArgumentException"> Thrown when the <paramref name="symbolicLink" /> value is not in a valid format. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="symbolicLink" /> is null. </exception>
        public static WindowsUsbDeviceInfo FromSymbolicLink(string symbolicLink)
        {
            if (symbolicLink == null)
            {
                throw new ArgumentNullException(nameof(symbolicLink));
            }

            if (!_validSymbolicLinkRegex.IsMatch(symbolicLink))
            {
                throw new ArgumentException("The value of the symbolic link is invalid.", nameof(symbolicLink));
            }

            // Example symbolic link format:
            // ##?#USB#VID_1C8A&PID_3A21#0000000000002#{28d78fad-5a12-11d1-ae5b-0000f803a8c2}
            // VID: Vendor ID
            // PID: Product ID
            // 0000000000002: Serial Number

            IDictionary<string, string> ids = _getIdFromDevicePathRegex.Matches(symbolicLink)
                .Cast<Match>()
                .Select(match => match.Value.Split('_'))
                .ToDictionary(value => value[0], value => value[value.Length - 1]);

            string pid = ids.First(id => id.Key.Equals(nameof(pid), StringComparison.OrdinalIgnoreCase)).Value;
            string vid = ids.First(id => id.Key.Equals(nameof(vid), StringComparison.OrdinalIgnoreCase)).Value;

            string[] deviceValues = symbolicLink.Split('#');
            string serialNumber = deviceValues[deviceValues.Length - 2];

            return new WindowsUsbDeviceInfo(pid, vid, symbolicLink, serialNumber);
        }
    }
}
