using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Usb.Net.Abstractions;
using Usb.Net.Abstractions.Events;
using Usb.Net.Avalonia.Abstractions.Views;
using Usb.Net.Avalonia.ViewModels.Abstractions;

namespace Usb.Net.Avalonia.ViewModels.Pages
{
    /// <summary>
    /// ViewModel for the UsbMonitorPage.
    /// </summary>
    public sealed partial class UsbMonitorPageViewModel : PageViewModelBase
    {
        private readonly IMainThread _mainThread;
        private readonly IUsbMonitor _usbMonitor;

        [ObservableProperty]
        private bool _isMonitoring;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbMonitorPageViewModel" /> class.
        /// </summary>
        /// <param name="mainThread"> Service to invoke actions on the main UI thread. </param>
        /// <param name="usbMonitor"> Service responsible for monitoring USB device events. </param>
        /// <param name="navigator"> Navigation service used to switch between pages. </param>
        public UsbMonitorPageViewModel(IMainThread mainThread, IUsbMonitor usbMonitor, INavigator navigator) : base(navigator)
        {
            _mainThread = mainThread;
            _usbMonitor = usbMonitor;
            Logs = new ObservableCollection<string>();
        }

        /// <summary>
        /// Gets the log of USB device events, including timestamps and device information.
        /// </summary>
        public ObservableCollection<string> Logs { get; }

        /// <inheritdoc cref="ViewModelBase.OnUnloaded" />
        public override void OnUnloaded()
        {
            OnStopMonitoring();
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            Navigator.Pop();
        }

        [RelayCommand]
        private void OnStartMonitoring()
        {
            _usbMonitor.StartMonitoring();

            _usbMonitor.DeviceAttached += OnDeviceAttached;
            _usbMonitor.DeviceDetached += OnDeviceDetached;

            IsMonitoring = _usbMonitor.IsMonitoring;
        }

        [RelayCommand]
        private void OnStopMonitoring()
        {
            _usbMonitor.StopMonitoring();

            _usbMonitor.DeviceAttached -= OnDeviceAttached;
            _usbMonitor.DeviceDetached -= OnDeviceDetached;

            IsMonitoring = _usbMonitor.IsMonitoring;
        }

        [RelayCommand]
        private void OnClearLog()
        {
            _mainThread.Invoke(Logs.Clear);
            IsMonitoring = _usbMonitor.IsMonitoring;
        }

        private void OnDeviceAttached(object sender, UsbDeviceChangedEventArgs e)
        {
            _mainThread.Invoke(() => Logs.Add($"[{GetCurrentTime()}] Device Attached: {e.DeviceInfo}"));
        }

        private void OnDeviceDetached(object sender, UsbDeviceChangedEventArgs e)
        {
            _mainThread.Invoke(() => Logs.Add($"[{GetCurrentTime()}] Device Detached: {e.DeviceInfo}"));
        }

        private static string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
        }
    }
}
