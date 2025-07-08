using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Usb.Net.Abstractions;
using Usb.Net.Abstractions.Exceptions;
using Usb.Net.Internals.Extensions;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace Usb.Net.Windows.Internals
{
    internal sealed partial class WindowsUsbDevice : IUsbDevice
    {
        /// <summary>
		/// <para> USB 1.1 WriteFile maximum block size is 4096. </para>
		/// <para> USB 1.1 ReadFile in block chunks of 64 bytes. </para>
		/// <para> USB 2.0 ReadFile in block chunks of 512 bytes. </para>
		/// </summary>
		/// <seealso href="https://www.usb.org/documents" />
		private const int BufferSize = 1 << 12;

        /// <summary>
		/// 기본 재시도 횟수를 가져옵니다.
		/// </summary>
		private const int DefaultRetry = 1;

        private readonly ILogger _logger;
        private readonly UsbDeviceInfo _deviceInfo;
        private bool _disposedValue;
        private SafeFileHandle? _handle;

        /// <summary>
		/// <see cref="WindowsUsbDevice" /> 를 초기화합니다.
		/// </summary>
		/// <param name="logger"> Log 를 작성할 수 있는 <see cref="ILogger{TCategoryName}" /> 의 구현체입니다. </param>
		/// <param name="deviceInfo"> USB 장치 정보입니다. </param>
		public WindowsUsbDevice(ILogger<WindowsUsbDevice> logger, UsbDeviceInfo deviceInfo)
        {
            _logger = logger;
            _deviceInfo = deviceInfo;
        }

        /// <inheritdoc cref="IUsbDevice.IsOpen" />
        public bool IsOpen => _handle?.IsClosed == false && !_handle.IsInvalid;

        /// <inheritdoc cref="IUsbDevice.Open" />
        public void Open()
        {
            if (!IsOpen)
            {
                try
                {
                    ThrowIfObjectDisposed();
                    _handle = CreateHandle(_deviceInfo);
                }
                catch (Exception e)
                {
                    throw new UsbHandleException("Failed to open the USB Handle.", e);
                }
            }
        }

        /// <inheritdoc cref="IUsbDevice.Close" />
        public void Close()
        {
            ThrowIfObjectDisposed();

            if (IsOpen)
            {
                _handle?.Close();
                _handle?.Dispose();

                _handle = null;
            }
        }
        public bool Write(params byte[] data)
        {
            return Write(IOTimeoutOptions.DefaultTimeout, data);
        }

        public bool Write(int retry, params byte[] data)
        {
            return Write(retry, IOTimeoutOptions.DefaultTimeout, data);
        }

        public bool Write(TimeSpan timeout, params byte[] data)
        {
            return Write(DefaultRetry, timeout, data);
        }

        public bool Write(int retry, TimeSpan timeout, params byte[] data)
        {
            ThrowIfObjectDisposed();
            ThrowArgumentOutOfRangeExceptionIfLessThan(retry, DefaultRetry);

            bool isWriteSuccess = false;
            uint dwMilliseconds = Convert.ToUInt32(timeout.TotalMilliseconds);

            for (int tryCount = 0; !isWriteSuccess && tryCount < retry; tryCount++)
            {
                isWriteSuccess = WriteCore(data, dwMilliseconds);
            }

            return isWriteSuccess;
        }

        public ReadOnlyMemory<byte> Read()
        {
            return Read(IOTimeoutOptions.DefaultTimeout);
        }

        public ReadOnlyMemory<byte> Read(int retry)
        {
            return Read(retry, IOTimeoutOptions.DefaultTimeout);
        }

        public ReadOnlyMemory<byte> Read(TimeSpan timeout)
        {
            return Read(DefaultRetry, timeout);
        }

        public ReadOnlyMemory<byte> Read(int retry, TimeSpan timeout)
        {
            ThrowIfObjectDisposed();
            ThrowArgumentOutOfRangeExceptionIfLessThan(retry, DefaultRetry);

            ReadOnlyMemory<byte> response = ReadOnlyMemory<byte>.Empty;
            uint dwMilliseconds = Convert.ToUInt32(timeout.TotalMilliseconds);

            for (int tryCount = 0; response.IsEmpty && tryCount < retry; tryCount++)
            {
                response = ReadCore(dwMilliseconds, ReadOnlySpan<byte>.Empty);
            }

            return response;
        }

        public ReadOnlyMemory<byte> WriteThenRead(params byte[] data)
        {
            return WriteThenRead(IOTimeoutOptions.DefaultTimeout, data);
        }

        public ReadOnlyMemory<byte> WriteThenRead(int retry, params byte[] data)
        {
            return WriteThenRead(retry, IOTimeoutOptions.DefaultTimeout, data);
        }

        public ReadOnlyMemory<byte> WriteThenRead(TimeSpan timeout, params byte[] data)
        {
            return WriteThenRead(DefaultRetry, timeout, data);
        }

        public ReadOnlyMemory<byte> WriteThenRead(int retry, TimeSpan timeout, params byte[] data)
        {
            ThrowIfObjectDisposed();
            ThrowArgumentOutOfRangeExceptionIfLessThan(retry, DefaultRetry);

            uint dwMilliseconds = Convert.ToUInt32(timeout.TotalMilliseconds);
            ReadOnlyMemory<byte> response = ReadOnlyMemory<byte>.Empty;
            for (int tryCount = 0; response.IsEmpty && tryCount < retry; tryCount++)
            {
                bool isWrited = WriteCore(data, dwMilliseconds);
                if (isWrited)
                {
                    response = ReadCore(dwMilliseconds, data);
                }
            }

            return response;
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
                    Close();
                }

                _disposedValue = true;
            }
        }

        private static unsafe SafeFileHandle? CreateHandle(UsbDeviceInfo deviceInfo)
        {
            if (string.IsNullOrEmpty(deviceInfo.Path))
            {
                return null;
            }

            return PInvoke.CreateFile(
                deviceInfo.Path,
                (uint)FILE_ACCESS_RIGHTS.FILE_GENERIC_READ | (uint)FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE,
                FILE_SHARE_MODE.FILE_SHARE_READ,
                null,
                FILE_CREATION_DISPOSITION.OPEN_ALWAYS,
                FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_SEQUENTIAL_SCAN | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_OVERLAPPED,
                null);
        }

        private unsafe bool WriteCore(ReadOnlySpan<byte> lpBuffer, uint dwMilliseconds)
        {
            bool isSuccess = false;
            if (!IsOpen)
            {
                throw new UsbConnectionRequiredException($"The [{nameof(Open)}] function must be called before write operation.");
            }

            bool isReferenceAdded = false;
            using SafeFileHandle eventHandle = CreateEvent();
            NativeOverlapped overlapped = CreateNativeOverlapped();
            IOTimeoutOptions options = IOTimeoutOptions.FromTotalDuration(dwMilliseconds);

            try
            {
                eventHandle.DangerousAddRef(ref isReferenceAdded);
                if (isReferenceAdded)
                {
                    uint bufferLength = (uint)lpBuffer.Length;
                    overlapped.EventHandle = eventHandle.DangerousGetHandle();

                    isSuccess = PInvoke.WriteFile(_handle, lpBuffer, &bufferLength, &overlapped);
                    if (!isSuccess)
                    {
                        WAIT_EVENT waitEvent = WAIT_EVENT.WAIT_OBJECT_0;
                        WIN32_ERROR lastError = GetLastError();
                        if (lastError == WIN32_ERROR.ERROR_IO_PENDING)
                        {
                            PInvoke.Sleep((uint)options.Delay.TotalMilliseconds);

                            waitEvent = PInvoke.WaitForSingleObject(eventHandle, (uint)options.Timeout.TotalMilliseconds);
                            isSuccess = waitEvent == WAIT_EVENT.WAIT_OBJECT_0 && GetOverlappedResult(_handle, overlapped, out int _) && PInvoke.ResetEvent(eventHandle);
                        }

                        if (isSuccess)
                        {
                            LogSuccessfullyWritten(lpBuffer);
                        }
                        else if (PInvoke.CancelIo(_handle))
                        {
                            ThrowTaskCanceledException(waitEvent, dwMilliseconds, lpBuffer);
                        }
                    }
                }
            }
            catch (TaskCanceledException e)
            {
                LogWriteCanceled(e);
            }
            finally
            {
                if (isReferenceAdded)
                {
                    overlapped.EventHandle = IntPtr.Zero;
                    eventHandle.DangerousRelease();
                }
            }

            return isSuccess;
        }

        private unsafe ReadOnlyMemory<byte> ReadCore(uint dwMilliseconds, ReadOnlySpan<byte> writtenData)
        {
            bool isSuccess = false;
            ReadOnlyMemory<byte> response = ReadOnlyMemory<byte>.Empty;

            if (!IsOpen)
            {
                throw new UsbConnectionRequiredException($"The [{nameof(Open)}] function must be called before read operation.");
            }

            bool isReferenceAdded = false;
            using SafeFileHandle eventHandle = CreateEvent();
            NativeOverlapped overlapped = CreateNativeOverlapped();
            IOTimeoutOptions options = IOTimeoutOptions.FromTotalDuration(dwMilliseconds);

            try
            {
                eventHandle.DangerousAddRef(ref isReferenceAdded);
                if (isReferenceAdded)
                {
                    uint lpNumberOfBytesRead;
                    Span<byte> lpBuffer = new byte[BufferSize];

                    int maxLength = lpBuffer.Length;
                    overlapped.EventHandle = eventHandle.DangerousGetHandle();

                    isSuccess = PInvoke.ReadFile(_handle, lpBuffer, &lpNumberOfBytesRead, &overlapped);
                    if (!isSuccess)
                    {
                        WAIT_EVENT waitEvent = WAIT_EVENT.WAIT_OBJECT_0;
                        WIN32_ERROR lastError = GetLastError();
                        if (lastError == WIN32_ERROR.ERROR_IO_PENDING)
                        {
                            PInvoke.Sleep((uint)options.Delay.TotalMilliseconds);

                            waitEvent = PInvoke.WaitForSingleObject(eventHandle, (uint)options.Timeout.TotalMilliseconds);
                            isSuccess = waitEvent == WAIT_EVENT.WAIT_OBJECT_0 && GetOverlappedResult(_handle, overlapped, out maxLength) && PInvoke.ResetEvent(eventHandle);
                        }

                        if (!isSuccess && PInvoke.CancelIo(_handle))
                        {
                            ThrowTaskCanceledException(waitEvent, dwMilliseconds, writtenData);
                        }
                    }

                    Span<byte> responseSpan = lpBuffer.Slice(default, maxLength).Trim(byte.MinValue);
                    response = responseSpan.ToArray();

                    LogSuccessfullyRead(response.Span);
                }
            }
            catch (TaskCanceledException e)
            {
                LogReadCanceled(e);
            }
            finally
            {
                if (isReferenceAdded)
                {
                    overlapped.EventHandle = IntPtr.Zero;
                    eventHandle.DangerousRelease();
                }
            }

            return response;
        }

        private void ThrowIfObjectDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(WindowsUsbDevice));
            }
        }

        private static void ThrowTaskCanceledException(WAIT_EVENT waitEvent, uint dwMilliseconds, ReadOnlySpan<byte> data, [CallerMemberName] string operationMethodName = "")
        {
            bool isWriteOperation = operationMethodName == nameof(WriteCore);
            string operationType = isWriteOperation ? "write" : "read";
            string message = $"During the {operationType} operation, waited for {dwMilliseconds} milliseconds, but failed because WAIT_EVENT was {waitEvent}, leading to a CancelIO call.";

            if (!data.IsEmpty)
            {
                string hexString = data.ToHexString();
                string loggingMessage = isWriteOperation ? $" The data that was intended to be written is [{hexString}]." : $" The data written was [{hexString}].";
                message = string.Concat(message, loggingMessage);
            }

            throw new TaskCanceledException(message);
        }

        private static void ThrowArgumentOutOfRangeExceptionIfLessThan(int value, int other)
        {
            if (value < other)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        private static NativeOverlapped CreateNativeOverlapped()
        {
            return new NativeOverlapped()
            {
                OffsetLow = byte.MinValue,
                OffsetHigh = byte.MinValue,
            };
        }

        private static SafeFileHandle CreateEvent()
        {
            return PInvoke.CreateEvent(null, true, false, string.Empty);
        }

        private static bool GetOverlappedResult(SafeHandle? handle, NativeOverlapped overlapped, out int transferred)
        {
            bool result = PInvoke.GetOverlappedResult(handle, overlapped, out uint lpNumberOfBytesTransferred, false);
            transferred = (int)lpNumberOfBytesTransferred;
            return result;
        }

        private static WIN32_ERROR GetLastError()
        {
            int win32Error = Marshal.GetLastWin32Error();
            return (WIN32_ERROR)win32Error;
        }

        private void LogSuccessfullyWritten(ReadOnlySpan<byte> data)
        {
            string hexString = data.ToHexString();
            LogWriteSuccess(hexString);
        }

        private void LogSuccessfullyRead(ReadOnlySpan<byte> data)
        {
            string hexString = data.ToHexString();
            LogReadSuccess(hexString);
        }

        [LoggerMessage(LogLevel.Information, "Successfully written command [{HexString}] to the device.")]
        private partial void LogWriteSuccess(string hexString);

        [LoggerMessage(LogLevel.Information, "Successfully received an response [{HexString}] following a read request to the device.")]
        private partial void LogReadSuccess(string hexString);

        [LoggerMessage(LogLevel.Error, "The operation was canceled while reading.")]
        private partial void LogReadCanceled(TaskCanceledException exception);

        [LoggerMessage(LogLevel.Error, "The operation was canceled while writing.")]
        private partial void LogWriteCanceled(TaskCanceledException exception);
    }
}
