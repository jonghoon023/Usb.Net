using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Usb.Net.Abstractions;
using Usb.Net.Windows.Internals.Win32Api;
using Usb.Net.Windows.Internals.Win32Api.Structures;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace Usb.Net.Windows.Internals
{
    /// <summary>
    /// Implementation of <see cref="IWindowsUsbDeviceProvider" />.
    /// </summary>
    internal sealed unsafe partial class WindowsUsbDeviceProvider : IWindowsUsbDeviceProvider
    {
        private const SETUP_DI_GET_CLASS_DEVS_FLAGS Flags = SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_DEVICEINTERFACE | SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT;

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private bool _disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsUsbDeviceProvider" /> class with the specified <see cref="ILoggerFactory" /> for logging support.
        /// </summary>
        /// <param name="loggerFactory"> The logger factory used to create loggers. </param>
        public WindowsUsbDeviceProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<WindowsUsbDeviceProvider>();
        }

        /// <inheritdoc cref="IWindowsUsbDeviceProvider.EnumerateDevices(Guid, CancellationToken)" />
        public IEnumerable<WindowsUsbDeviceInfo> EnumerateDevices(Guid deviceInterfaceClass, CancellationToken cancellationToken = default)
        {
            List<WindowsUsbDeviceInfo> deviceInfos = new();
            using (SetupDiDestroyDeviceInfoListSafeHandle deviceInfoList = PInvoke.SetupDiGetClassDevs(deviceInterfaceClass, string.Empty, HWND.Null, Flags))
            {
                if (!deviceInfoList.IsInvalid && !deviceInfoList.IsClosed)
                {
                    uint memberIndex = 0;
                    while (!cancellationToken.IsCancellationRequested && SetupApi.SetupDiEnumDeviceInterfaces(deviceInfoList, ref deviceInterfaceClass, memberIndex, out SP_DEVICE_INTERFACE_DATA deviceInterfaceData))
                    {
                        SP_DEVINFO_DATA devinfoData = new()
                        {
                            cbSize = (uint)sizeof(SP_DEVINFO_DATA),
                        };

                        IncrementUInt(ref memberIndex);

                        string devicePath = SetupDiGetDeviceInterfaceDetail(deviceInfoList, deviceInterfaceData, &devinfoData);
                        deviceInfos.Add(WindowsUsbDeviceInfo.FromSymbolicLink(devicePath));
                        LogDevicePathFound(devicePath);
                    }
                }
            }

            return deviceInfos;
        }

        /// <inheritdoc cref="IUsbDeviceProvider.EnumerateDevices(CancellationToken)" />
        public IEnumerable<UsbDeviceInfo> EnumerateDevices(CancellationToken cancellationToken = default)
        {
            return EnumerateDevices(PInvoke.GUID_DEVINTERFACE_USB_DEVICE, cancellationToken);
        }

        /// <inheritdoc cref="IUsbDeviceProvider.CreateDevice(UsbDeviceInfo)" />
        public IUsbDevice CreateDevice(UsbDeviceInfo deviceInfo)
        {
            ILogger<WindowsUsbDevice> logger = _loggerFactory.CreateLogger<WindowsUsbDevice>();
            return new WindowsUsbDevice(logger, deviceInfo);
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _loggerFactory.Dispose();
                }

                _disposedValue = true;
            }
        }

        private static string SetupDiGetDeviceInterfaceDetail(SafeHandle handle, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, SP_DEVINFO_DATA* deviceInfoData)
        {
            uint requiredSize;
            bool isFirst = SetupApi.SetupDiGetDeviceInterfaceDetail(handle, ref deviceInterfaceData, null, default, &requiredSize, deviceInfoData);
            int devicePathOffset = (int)Marshal.OffsetOf<SP_DEVICE_INTERFACE_DETAIL_DATA>(nameof(SP_DEVICE_INTERFACE_DETAIL_DATA.DevicePath));

            WIN32_ERROR lastError = (WIN32_ERROR)Marshal.GetLastWin32Error();
            if (!isFirst && lastError == WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
            {
                byte[] buffer = new byte[requiredSize];
                fixed (byte* pBuffer = buffer)
                {
                    SP_DEVICE_INTERFACE_DETAIL_DATA* deviceInterfaceDetailData = (SP_DEVICE_INTERFACE_DETAIL_DATA*)pBuffer;
                    deviceInterfaceDetailData->cbSize = (uint)SP_DEVICE_INTERFACE_DETAIL_DATA.ReportableStructSize;

                    bool isSecond = SetupApi.SetupDiGetDeviceInterfaceDetail(handle, ref deviceInterfaceData, deviceInterfaceDetailData, requiredSize, default, deviceInfoData);
                    if (isSecond)
                    {
                        byte[] bytes = new ArraySegment<byte>(buffer, devicePathOffset, buffer.Length - devicePathOffset).Where(b => b != byte.MinValue).ToArray();
                        return Encoding.Default.GetString(bytes).Trim(char.MinValue);
                    }
                }
            }

            return string.Empty;
        }

        private static void IncrementUInt(ref uint location)
        {
            int locationInt = (int)location;
            Interlocked.Increment(ref locationInt);

            location = (uint)locationInt;
        }

        [LoggerMessage(LogLevel.Information, "Device path found: {DevicePath}")]
        private partial void LogDevicePathFound(string devicePath);
    }
}
