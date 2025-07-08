using System;
using System.Runtime.InteropServices;

namespace Usb.Net.Windows.Internals.Win32Api.Structures
{
    /// <summary>
    /// An SP_DEVICE_INTERFACE_DATA structure defines a device interface in a device information set.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/ko-kr/windows/win32/api/setupapi/ns-setupapi-sp_device_interface_data" />
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SP_DEVICE_INTERFACE_DATA : IEquatable<SP_DEVICE_INTERFACE_DATA>
    {
        /// <summary>
        /// The size, in bytes, of the SP_DEVICE_INTERFACE_DATA structure. For more information, see the Remarks section.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// The GUID for the class to which the device interface belongs.
        /// </summary>
        public Guid InterfaceClassGuid;

        /// <summary>
        /// Can be one or more of the following
        /// </summary>
        /// <remarks>
        /// SPINT_ACTIVE <br />
        /// SPINT_DEFAULT <br />
        /// SPINT_REMOVED
        /// </remarks>
        public uint Flags;

        /// <summary>
        /// Reserved. Do not use.
        /// </summary>
        internal IntPtr Reserved;

        /// <inheritdoc cref="object.GetHashCode" />
        public readonly override int GetHashCode()
        {
            return Tuple.Create(cbSize, InterfaceClassGuid, Flags).GetHashCode();
        }

        /// <inheritdoc cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return obj is SP_DEVICE_INTERFACE_DATA other && Equals(other);
        }

        /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
        public bool Equals(SP_DEVICE_INTERFACE_DATA other)
        {
            return 
                cbSize == other.cbSize &&
                InterfaceClassGuid.Equals(other.InterfaceClassGuid) &&
                Flags == other.Flags;
        }

        /// <summary>
        /// Determines whether two <see cref="SP_DEVICE_INTERFACE_DATA" /> instances are equal.
        /// </summary>
        /// <param name="left"> The first <see cref="SP_DEVICE_INTERFACE_DATA" /> to compare. </param>
        /// <param name="right"> The second <see cref="SP_DEVICE_INTERFACE_DATA" /> to compare. </param>
        /// <returns> <see langword="true" /> if the two instances are equal; otherwise, <see langword="false" />. </returns>
        public static bool operator ==(SP_DEVICE_INTERFACE_DATA left, SP_DEVICE_INTERFACE_DATA right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="SP_DEVICE_INTERFACE_DATA" /> instances are not equal.
        /// </summary>
        /// <param name="left"> The first <see cref="SP_DEVICE_INTERFACE_DATA"/> to compare. </param>
        /// <param name="right"> The second <see cref="SP_DEVICE_INTERFACE_DATA"/> to compare. </param>
        /// <returns> <see langword="true" /> if the two instances are not equal; otherwise, <see langword="false" />. </returns>
        public static bool operator !=(SP_DEVICE_INTERFACE_DATA left, SP_DEVICE_INTERFACE_DATA right)
        {
            return !(left == right);
        }
    }
}
