using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using Usb.Net.Abstractions;
using Usb.Net.Windows.Internals;

namespace Usb.Net.Extensions
{
    /// <summary>
    /// Static class that contains extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers USB-related services based on the current operating system.
        /// </summary>
        /// <param name="services"> The service collection to which the USB services will be added. </param>
        /// <returns> The updated service collection. </returns>
        public static IServiceCollection AddUsb(this IServiceCollection services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.AddTransient<IUsbMonitor, WindowsUsbMonitor>();
                services.AddTransient<IUsbDeviceProvider, WindowsUsbDeviceProvider>();
            }

            return services;
        }
    }
}
