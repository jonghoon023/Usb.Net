using System;
using System.Runtime.InteropServices;

namespace Usb.Net.Windows.Internals.Win32Api.Structures
{
    /// <summary>
    /// An SP_DEVICE_INTERFACE_DETAIL_DATA structure contains the path for a device interface.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/ko-kr/windows/win32/api/setupapi/ns-setupapi-sp_device_interface_detail_data_w" />
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal unsafe struct SP_DEVICE_INTERFACE_DETAIL_DATA : IEquatable<SP_DEVICE_INTERFACE_DETAIL_DATA>
    {
        /// <summary>
        /// The size, in bytes, of the SP_DEVICE_INTERFACE_DETAIL_DATA structure. For more information, see the following Remarks section.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// A NULL-terminated string that contains the device interface path. This path can be passed to Win32 functions such as CreateFile.
        /// </summary>
        public fixed char DevicePath[1];

        /// <summary>
        /// Gets the value that should be set to the <see cref="cbSize"/> field.
        /// </summary>
        /// <remarks>
        /// The structure size take into account an Int32 and a character.
        /// The character can be 1 or 2 octets depending on ANSI / Unicode,
        /// leading to a total struct size of 5 or 6 bytes.
        /// But on 64-bit OS, we report 8 bytes due to memory alignment.
        /// </remarks>
        public static int ReportableStructSize => IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8;

        /// <summary>
        /// Extracts the device path string from the fixed buffer.
        /// </summary>
        public string GetDevicePath()
        {
            fixed (char* devicePathPtr = DevicePath)
            {
                return new string(devicePathPtr).Trim();
            }
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            return cbSize.GetHashCode();
        }

        /// <inheritdoc cref="object.Equals(object)" />
        public readonly override bool Equals(object obj)
        {
            return obj is SP_DEVICE_INTERFACE_DETAIL_DATA other && Equals(other);
        }

        /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
        public readonly bool Equals(SP_DEVICE_INTERFACE_DETAIL_DATA other)
        {
            return cbSize == other.cbSize;
        }

        /// <summary>
        /// Determines whether two <see cref="SP_DEVICE_INTERFACE_DETAIL_DATA" /> instances are equal.
        /// </summary>
        /// <param name="left"> The first instance to compare. </param>
        /// <param name="right"> The second instance to compare. </param>
        /// <returns> <see langword="true" /> if both instances are equal; otherwise, <see langword="false" />. </returns>
        public static bool operator ==(SP_DEVICE_INTERFACE_DETAIL_DATA left, SP_DEVICE_INTERFACE_DETAIL_DATA right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="SP_DEVICE_INTERFACE_DETAIL_DATA" /> instances are not equal.
        /// </summary>
        /// <param name="left"> The first instance to compare. </param>
        /// <param name="right"> The second instance to compare. </param>
        /// <returns> <see langword="true" /> if the instances are not equal; otherwise, <see langword="false" />. </returns>
        public static bool operator !=(SP_DEVICE_INTERFACE_DETAIL_DATA left, SP_DEVICE_INTERFACE_DETAIL_DATA right)
        {
            return !(left == right);
        }
    }
}
