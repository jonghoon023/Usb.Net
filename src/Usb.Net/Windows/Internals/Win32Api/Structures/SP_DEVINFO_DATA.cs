using System;
using System.Runtime.InteropServices;

namespace Usb.Net.Windows.Internals.Win32Api.Structures
{
    /// <summary>
    /// An SP_DEVINFO_DATA structure defines a device instance that is a member of a device information set.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/ko-kr/windows/win32/api/setupapi/ns-setupapi-sp_devinfo_data" />
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SP_DEVINFO_DATA : IEquatable<SP_DEVINFO_DATA>
    {
        /// <summary>
        /// The size, in bytes, of the SP_DEVINFO_DATA structure. For more information, see the following Remarks section.
        /// </summary>
        internal uint cbSize;

        /// <summary>
        /// The GUID of the device's setup class.
        /// </summary>
        public Guid ClassGuid;

        /// <summary>
        /// An opaque handle to the device instance (also known as a handle to the devnode).
        /// </summary>
        /// <remarks> Some functions, such as SetupDiXxx functions, take the whole SP_DEVINFO_DATA structure as input to identify a device in a device information set. Other functions, such as CM_Xxx functions like CM_Get_DevNode_Status, take this DevInst handle as input. </remarks>
        public uint DevInst;

        /// <summary>
        /// Reserved. For internal use only.
        /// </summary>
        internal IntPtr Reserved;

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            return Tuple.Create(ClassGuid, DevInst).GetHashCode();
        }

        /// <inheritdoc cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return obj is SP_DEVINFO_DATA other && Equals(other);
        }

        /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
        public bool Equals(SP_DEVINFO_DATA other)
        {
            return ClassGuid.Equals(other.ClassGuid) && DevInst == other.DevInst;
        }

        /// <summary>
        /// Determines whether two <see cref="SP_DEVINFO_DATA" /> instances are equal.
        /// </summary>
        /// <param name="left"> The first instance to compare. </param>
        /// <param name="right"> The second instance to compare. </param>
        /// <returns> <see langword="true" /> if both instances are equal; otherwise, <see langword="false" />. </returns>
        public static bool operator ==(SP_DEVINFO_DATA left, SP_DEVINFO_DATA right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="SP_DEVINFO_DATA" /> instances are not equal.
        /// </summary>
        /// <param name="left"> The first instance to compare. </param>
        /// <param name="right"> The second instance to compare. </param>
        /// <returns> <see langword="true" /> if the instances are not equal; otherwise, <see langword="false" />. </returns>
        public static bool operator !=(SP_DEVINFO_DATA left, SP_DEVINFO_DATA right)
        {
            return !(left == right);
        }
    }
}
