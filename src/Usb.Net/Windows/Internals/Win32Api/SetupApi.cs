using System;
using System.Runtime.InteropServices;
using Usb.Net.Windows.Internals.Win32Api.Structures;

namespace Usb.Net.Windows.Internals.Win32Api
{
    internal static class SetupApi
    {
        private const string SetupAPiLibrary = "setupapi.dll";

        /// <summary>
        /// The SetupDiEnumDeviceInterfaces function enumerates the device interfaces that are contained in a device information set.
        /// </summary>
        /// <param name="deviceInfoSet"> A pointer to a device information set that contains the device interfaces for which to return information. This handle is typically returned by SetupDiGetClassDevs. </param>
        /// <param name="interfaceClassGuid"> A pointer to a GUID that specifies the device interface class for the requested interface. </param>
        /// <param name="memberIndex">
        ///	A zero-based index into the list of interfaces in the device information set. The caller should call this function first with MemberIndex set to zero to obtain the first interface. Then, repeatedly increment MemberIndex and retrieve an interface until this function fails and GetLastError returns ERROR_NO_MORE_ITEMS. <br />
        ///	If DeviceInfoData specifies a particular device, the MemberIndex is relative to only the interfaces exposed by that device.
        /// </param>
        /// <param name="deviceInterfaceData"> A pointer to a caller-allocated buffer that contains, on successful return, a completed SP_DEVICE_INTERFACE_DATA structure that identifies an interface that meets the search parameters. The caller must set DeviceInterfaceData.cbSize to sizeof(SP_DEVICE_INTERFACE_DATA) before calling this function. </param>
        /// <returns> SetupDiEnumDeviceInterfaces returns TRUE if the function completed without error. If the function completed with an error, FALSE is returned and the error code for the failure can be retrieved by calling GetLastError. </returns>
        /// <seealso href="https://learn.microsoft.com/ko-kr/windows/win32/api/setupapi/nf-setupapi-setupdienumdeviceinterfaces" />
        public static unsafe bool SetupDiEnumDeviceInterfaces(SafeHandle deviceInfoSet, ref Guid interfaceClassGuid, uint memberIndex, out SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA()
            {
                cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>()
            };

            return SetupDiEnumDeviceInterfaces(deviceInfoSet.DangerousGetHandle(), null, ref interfaceClassGuid, memberIndex, ref deviceInterfaceData);
        }

        /// <summary>
        /// The SetupDiGetDeviceRegistryProperty function retrieves a specified Plug and Play device property.
        /// </summary>
        /// <param name="deviceInfoSet"> A handle to a device information set that contains a device information element that represents the device for which to retrieve a Plug and Play property. </param>
        /// <param name="deviceInfoData"> A pointer to an SP_DEVINFO_DATA structure that specifies the device information element in DeviceInfoSet. </param>
        /// <param name="property"> One of the following values that specifies the property to be retrieved </param>
        /// <param name="propertyBuffer"> A pointer to a buffer that receives the property that is being retrieved. If this parameter is set to NULL, and PropertyBufferSize is also set to zero, the function returns the required size for the buffer in RequiredSize. </param>
        /// <returns> SetupDiGetDeviceRegistryProperty returns TRUE if the call was successful. Otherwise, it returns FALSE and the logged error can be retrieved by making a call to GetLastError. SetupDiGetDeviceRegistryProperty returns the ERROR_INVALID_DATA error code if the requested property does not exist for a device or if the property data is not valid. </returns>
        /// <seealso href="https://learn.microsoft.com/ko-kr/windows/win32/api/setupapi/nf-setupapi-setupdigetdeviceregistrypropertya" />
        public static unsafe bool SetupDiGetDeviceRegistryProperty(SafeHandle deviceInfoSet, SP_DEVINFO_DATA* deviceInfoData, uint property, ref byte[] propertyBuffer)
        {
            fixed (byte* propertyBufferPointer = propertyBuffer)
            {
                return SetupDiGetDeviceRegistryProperty(deviceInfoSet.DangerousGetHandle(), deviceInfoData, property, out uint _, propertyBufferPointer, (uint)propertyBuffer.Length, out uint _);
            }
        }

        /// <summary>
        /// The SetupDiGetDeviceInterfaceDetail function returns details about a device interface.
        /// </summary>
        /// <param name="deviceInfoSet"> A pointer to the device information set that contains the interface for which to retrieve details. This handle is typically returned by SetupDiGetClassDevs. </param>
        /// <param name="deviceInterfaceData"> A pointer to an SP_DEVICE_INTERFACE_DATA structure that specifies the interface in DeviceInfoSet for which to retrieve details. A pointer of this type is typically returned by SetupDiEnumDeviceInterfaces. </param>
        /// <param name="deviceInterfaceDetailData"> A pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA structure to receive information about the specified interface. This parameter is optional and can be NULL. This parameter must be NULL if DeviceInterfaceDetailSize is zero. If this parameter is specified, the caller must set DeviceInterfaceDetailData.cbSize to sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA) before calling this function. The cbSize member always contains the size of the fixed part of the data structure, not a size reflecting the variable-length string at the end. </param>
        /// <param name="deviceInterfaceDetailDataSize">
        ///	The size of the DeviceInterfaceDetailData buffer. The buffer must be at least (offsetof(SP_DEVICE_INTERFACE_DETAIL_DATA, DevicePath) + sizeof(TCHAR)) bytes, to contain the fixed part of the structure and a single NULL to terminate an empty MULTI_SZ string. <br />
        ///	This parameter must be zero if DeviceInterfaceDetailData is NULL.
        /// </param>
        /// <param name="requiredSize"> A pointer to a variable of type DWORD that receives the required size of the DeviceInterfaceDetailData buffer. This size includes the size of the fixed part of the structure plus the number of bytes required for the variable-length device path string. This parameter is optional and can be NULL. </param>
        /// <param name="deviceInfoData"> A pointer to a buffer that receives information about the device that supports the requested interface. The caller must set DeviceInfoData.cbSize to sizeof(SP_DEVINFO_DATA). This parameter is optional and can be NULL. </param>
        /// <returns> SetupDiGetDeviceInterfaceDetail returns TRUE if the function completed without error. If the function completed with an error, FALSE is returned and the error code for the failure can be retrieved by calling GetLastError. </returns>
        /// <seealso href="https://learn.microsoft.com/ko-kr/windows/win32/api/setupapi/nf-setupapi-setupdigetdeviceinterfacedetailw" />
        public static unsafe bool SetupDiGetDeviceInterfaceDetail(SafeHandle deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, SP_DEVICE_INTERFACE_DETAIL_DATA* deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, uint* requiredSize, SP_DEVINFO_DATA* deviceInfoData)
        {
            bool deviceInfoSetAddRef = false;
            try
            {
                fixed (SP_DEVICE_INTERFACE_DATA* deviceInterfaceDataLocal = &deviceInterfaceData)
                {
                    deviceInfoSet.DangerousAddRef(ref deviceInfoSetAddRef);
                    IntPtr deviceInfoSetHandle = deviceInfoSet.DangerousGetHandle();

                    return SetupDiGetDeviceInterfaceDetail(deviceInfoSetHandle, deviceInterfaceDataLocal, deviceInterfaceDetailData, deviceInterfaceDetailDataSize, requiredSize, deviceInfoData);
                }
            }
            finally
            {
                if (deviceInfoSetAddRef)
                {
                    deviceInfoSet.DangerousRelease();
                }
            }
        }

        [DllImport(SetupAPiLibrary, EntryPoint = "SetupDiGetDeviceInterfaceDetailW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
        private static extern unsafe bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA* deviceInterfaceData, SP_DEVICE_INTERFACE_DETAIL_DATA* deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, uint* requiredSize, SP_DEVINFO_DATA* deviceInfoData);

        [DllImport(SetupAPiLibrary, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
        private static extern unsafe bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, SP_DEVINFO_DATA* deviceInfoData, ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport(SetupAPiLibrary, EntryPoint = "SetupDiGetDeviceRegistryPropertyA", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
        private static extern unsafe bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA* deviceInfoData, uint property, out uint propertyRegDataType, byte* propertyBuffer, uint propertyBufferSize, out uint requiredSize);
    }
}
