using Microsoft.Extensions.Logging;
using System;
using Usb.Net.Abstractions;
using Usb.Net.Abstractions.Events;
using Usb.Net.Abstractions.Exceptions;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;

namespace Usb.Net.Windows.Internals
{
    internal sealed unsafe partial class WindowsUsbMonitor : IWindowsUsbMonitor
    {
        private readonly ILogger _logger;
        private readonly PCM_NOTIFY_CALLBACK _callback;
		private CM_Unregister_NotificationSafeHandle? _notificationHandle;
        private bool _disposedValue;

        /// <summary>
		/// <see cref="WindowsUsbMonitor" /> 를 초기화합니다.
		/// </summary>
		/// <param name="loggerFactory"> <see cref="ILoggerFactory" /> 의 구현체입니다. </param>
		public unsafe WindowsUsbMonitor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WindowsUsbMonitor>();
            _callback = new PCM_NOTIFY_CALLBACK(Notification);
        }

        /// <inheritdoc cref="IUsbMonitor.DeviceAttached" />
        public event EventHandler<UsbDeviceChangedEventArgs>? DeviceAttached;

        /// <inheritdoc cref="IUsbMonitor.DeviceDetached" />
        public event EventHandler<UsbDeviceChangedEventArgs>? DeviceDetached;

        /// <inheritdoc cref="IUsbMonitor.IsMonitoring" />
        public bool IsMonitoring { get; set; }

        /// <inheritdoc cref="IUsbMonitor.StartMonitoring" />
        public void StartMonitoring()
        {
            StartMonitoring(PInvoke.GUID_DEVINTERFACE_USB_DEVICE);
        }

        /// <inheritdoc cref="IWindowsUsbMonitor.StartMonitoring(Guid)" />
        public void StartMonitoring(Guid deviceInterfaceClass)
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(WindowsUsbMonitor));
            }

            if (!IsMonitoring)
            {
                CM_NOTIFY_FILTER notifyFilter = new()
                {
                    cbSize = (uint)sizeof(CM_NOTIFY_FILTER),
                    FilterType = CM_NOTIFY_FILTER_TYPE.CM_NOTIFY_FILTER_TYPE_DEVICEINTERFACE
                };
                notifyFilter.u.DeviceInterface.ClassGuid = deviceInterfaceClass;

                CONFIGRET configret = PInvoke.CM_Register_Notification(notifyFilter, null, _callback, out CM_Unregister_NotificationSafeHandle? notificationHandle);
                if (configret != CONFIGRET.CR_SUCCESS)
                {
                    throw new UsbMonitoringException("Unable to detect PNP for USB device.");
                }

                _notificationHandle = notificationHandle;
                IsMonitoring = true;
            }

            LogMonitoringStarted(deviceInterfaceClass);
        }

        /// <inheritdoc cref="IUsbMonitor.StopMonitoring" />
        public void StopMonitoring()
        {
            if (IsMonitoring)
            {
                _notificationHandle?.Dispose();
                if (_notificationHandle?.IsClosed ?? true)
                {
                    IsMonitoring = false;
                    LogMonitoringStopped();
                }
            }
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
                    StopMonitoring();
                }

                _notificationHandle = null;
                _disposedValue = true;
            }
        }

        private uint Notification(HCMNOTIFICATION notify, void* context, CM_NOTIFY_ACTION action, CM_NOTIFY_EVENT_DATA* eventData, uint eventDataSize)
        {
            if (eventData->FilterType == CM_NOTIFY_FILTER_TYPE.CM_NOTIFY_FILTER_TYPE_DEVICEINTERFACE)
            {
                string symbolicLink = GetSymbolicLinkString(ref eventData->u.DeviceInterface.SymbolicLink);
                WindowsUsbDeviceInfo deviceSpecification = WindowsUsbDeviceInfo.FromSymbolicLink(symbolicLink);

                if (action == CM_NOTIFY_ACTION.CM_NOTIFY_ACTION_DEVICEINTERFACEARRIVAL)
                {
                    DeviceAttached?.Invoke(this, new UsbDeviceChangedEventArgs(deviceSpecification));
                    LogDeviceAttached(deviceSpecification);
                }
                else if (action == CM_NOTIFY_ACTION.CM_NOTIFY_ACTION_DEVICEINTERFACEREMOVAL)
                {
                    DeviceDetached?.Invoke(this, new UsbDeviceChangedEventArgs(deviceSpecification));
                    LogDeviceDetached(deviceSpecification);
                }

                LogSymbolicLinkReceived(symbolicLink);
                LogDeviceInfoParsed(deviceSpecification);
            }

            return default;
        }

        private static string GetSymbolicLinkString(ref VariableLengthInlineArray<char, ushort> symbolicLink)
        {
            fixed (ushort* symbolicLinkPointer = &symbolicLink.e0)
            {
                char* symbolicLinkCharPointer = (char*)symbolicLinkPointer;
                return new string(symbolicLinkCharPointer);
            }
        }

        [LoggerMessage(LogLevel.Information, "Detection of PNP devices started. (Device Interface Class: {DeviceInterfaceClass})")]
        private partial void LogMonitoringStarted(Guid deviceInterfaceClass);

        [LoggerMessage(LogLevel.Information, "Detection of PNP devices stopped.")]
        private partial void LogMonitoringStopped();

        [LoggerMessage(LogLevel.Information, "Device ({DeviceInfo}) interface arrival.")]
        private partial void LogDeviceAttached(WindowsUsbDeviceInfo deviceInfo);

        [LoggerMessage(LogLevel.Information, "Device ({DeviceInfo}) interface removal.")]
        private partial void LogDeviceDetached(WindowsUsbDeviceInfo deviceInfo);

        [LoggerMessage(LogLevel.Debug, "SymbolicLink: {SymbolicLink}")]
        private partial void LogSymbolicLinkReceived(string symbolicLink);

        [LoggerMessage(LogLevel.Debug, "{DeviceInfo}")]
        private partial void LogDeviceInfoParsed(WindowsUsbDeviceInfo deviceInfo);
    }
}
