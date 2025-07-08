using System;
using System.Globalization;

namespace Usb.Net.Abstractions
{
    /// <summary>
    /// Represents essential information about a USB device required for establishing communication.
    /// </summary>
    public class UsbDeviceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbDeviceInfo" /> class using numeric identifiers.
        /// </summary>
        /// <param name="productId"> The product ID that identifies the device model. </param>
        /// <param name="vendorId"> The vendor ID assigned to the device manufacturer. </param>
        /// <param name="path"> The platform-specific interface path used to access the device. </param>
        /// <param name="serialNumber"> The unique serial number of the USB device. </param>
        public UsbDeviceInfo(uint productId, uint vendorId, string path, string serialNumber)
        {
            ProductId = productId;
            VendorId = vendorId;
            Path = path;
            SerialNumber = serialNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbDeviceInfo" /> class using hexadecimal string identifiers.
        /// </summary>
        /// <param name="productId"> The product ID in hexadecimal string format (e.g., <c> "3A21" </c>). </param>
        /// <param name="vendorId"> The vendor ID in hexadecimal string format (e.g., <c> "1C8A" </c>). </param>
        /// <param name="path"> The platform-specific interface path used to access the device. </param>
        /// <param name="serialNumber"> The unique serial number of the USB device.</param>
        public UsbDeviceInfo(string productId, string vendorId, string path, string serialNumber)
        {
            if (uint.TryParse(productId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint pid))
            {
                ProductId = pid;
            }

            if (uint.TryParse(vendorId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint vid))
            {
                VendorId = vid;
            }

            Path = path;
            SerialNumber = serialNumber;
        }

        /// <summary>
        /// Gets the product ID that identifies the USB device model.
        /// </summary>
        public uint ProductId { get; }

        /// <summary>
        /// Gets the vendor ID assigned to the USB device manufacturer.
        /// </summary>
        public uint VendorId { get; }

        /// <summary>
        /// Gets the platform-specific device interface path used to access and communicate with the USB device.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the unique serial number of the USB device.
        /// </summary>
        public string SerialNumber { get; }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return $"UsbDeviceInfo [ProductId: {ProductId:X4}, VendorId: {VendorId:X4}, Path: {Path}, SerialNumber: {SerialNumber}]";
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            return Tuple.Create(ProductId, VendorId, Path, SerialNumber).GetHashCode();
        }

        /// <inheritdoc cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            if (obj is UsbDeviceInfo other)
            {
                return 
                    ProductId == other.ProductId && 
                    VendorId == other.VendorId && 
                    Path.Equals(other.Path, StringComparison.OrdinalIgnoreCase) && 
                    SerialNumber.Equals(other.SerialNumber, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
