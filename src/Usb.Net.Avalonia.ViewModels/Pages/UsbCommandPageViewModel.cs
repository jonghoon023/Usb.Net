using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Usb.Net.Abstractions;
using Usb.Net.Avalonia.Abstractions.Views;
using Usb.Net.Avalonia.Models;
using Usb.Net.Avalonia.ViewModels.Abstractions;

namespace Usb.Net.Avalonia.ViewModels.Pages
{
    /// <summary>
    /// ViewModel for the UsbCommandPage.
    /// </summary>
    public sealed partial class UsbCommandPageViewModel : PageViewModelBase
    {
        private readonly IMainThread _mainThread;
        private readonly IUsbDeviceProvider _usbDeviceProvider;

        private string? _hexString;

        [ObservableProperty]
        private UsbDeviceInfo? _deviceInfo;

        [ObservableProperty]
        private string? _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbCommandPageViewModel" /> class.
        /// </summary>
        /// <param name="mainThread"> Service for invoking actions on the UI thread. </param>
        /// <param name="usbDeviceProvider"> Service for creating and enumerating USB devices. </param>
        /// <param name="navigator"> Navigation service for page transitions. </param>
        public UsbCommandPageViewModel(IMainThread mainThread, IUsbDeviceProvider usbDeviceProvider, INavigator navigator) : base(navigator)
        {
            _mainThread = mainThread;
            _usbDeviceProvider = usbDeviceProvider;

            DeviceInfos = new ObservableCollection<UsbDeviceInfo>();
        }

        /// <summary>
        /// A collection of USB device information entries retrieved from the system.
        /// </summary>
        public ObservableCollection<UsbDeviceInfo> DeviceInfos { get; }

        /// <inheritdoc cref="ViewModelBase.OnInitializedAsync" />
        public override Task OnInitializedAsync()
        {
            return RefreshAsync();
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            Navigator.Pop();
        }

        [RelayCommand]
        private Task RefreshAsync()
        {
            return Task.Run(async () =>
            {
                await ClearDeviceInfosAsync().ConfigureAwait(false);
                foreach (UsbDeviceInfo deviceInfo in _usbDeviceProvider.EnumerateDevices())
                {
                    await _mainThread.InvokeAsync(() => DeviceInfos.Add(deviceInfo)).ConfigureAwait(false);
                }
            });
        }

        [RelayCommand]
        private void OnSelectionChanged(UsbDeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        [RelayCommand]
        private void OnTextChanged(string hexString)
        {
            _hexString = hexString;
        }

        [RelayCommand]
        private async Task WriteAsync()
        {
            string result = string.Empty;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                using IUsbDevice device = CreateDevice();
                device.Open();

                if (device.IsOpen)
                {
                    byte[] command = HexConverter.ToBytes(_hexString);
                    string hexString = HexConverter.ToHexString(command);

                    bool isSent = await Task.Run(() => device.Write(command)).ConfigureAwait(false);
                    result = $"Sent data: {hexString}\n\nResult: {isSent}";
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                Result = result;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        [RelayCommand]
        private async Task ReadAsync()
        {
            string result = string.Empty;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                using IUsbDevice device = CreateDevice();
                device.Open();

                if (device.IsOpen)
                {
                    ReadOnlyMemory<byte> response = await Task.Run(device.Read).ConfigureAwait(false);
                    byte[] responseBytes = response.ToArray();

                    string responseHexString = HexConverter.ToHexString(responseBytes);
                    string responseDecodedString = DecodeBytes(responseBytes);

                    result = $"Read result: {responseHexString}\n\nDecoded result: {responseDecodedString}";
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                Result = result;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        [RelayCommand]
        private async Task WriteThenReadAsync()
        {
            string result = string.Empty;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                using IUsbDevice device = CreateDevice();
                device.Open();

                if (device.IsOpen)
                {
                    byte[] command = HexConverter.ToBytes(_hexString);
                    string commandHexString = HexConverter.ToHexString(command);

                    ReadOnlyMemory<byte> response = await Task.Run(() => device.WriteThenRead(command)).ConfigureAwait(false);
                    byte[] responseBytes = response.ToArray();

                    string responseHexString = HexConverter.ToHexString(responseBytes);
                    string responseDecodedString = DecodeBytes(responseBytes);

                    result = $"Sent data: {commandHexString}\n\nRead result: {responseHexString}\n\nDecoded result: {responseDecodedString}";
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                Result = result;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private Task ClearDeviceInfosAsync()
        {
            DeviceInfo = null;
            return _mainThread.InvokeAsync(DeviceInfos.Clear);
        }

        private static string DecodeBytes(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        private IUsbDevice CreateDevice()
        {
            if (DeviceInfo == null)
            {
                throw new InvalidOperationException($"Cannot create a device because {nameof(DeviceInfo)} is null.");
            }

            return _usbDeviceProvider.CreateDevice(DeviceInfo);
        }
    }
}
