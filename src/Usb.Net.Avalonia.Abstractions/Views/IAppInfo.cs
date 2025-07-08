using System;

namespace Usb.Net.Avalonia.Abstractions.Views
{
    /// <summary>
    /// Service that provides information about the application.
    /// </summary>
    public interface IAppInfo
    {
        /// <summary>
        /// Gets whether the application is packaged.
        /// </summary>
        bool IsPackaged { get; }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the path to the cache directory.
        /// </summary>
        string CacheDirectory { get; }

        /// <summary>
        /// Gets the path to the AppData directory.
        /// </summary>
        string AppDataDirectory { get; }
    }
}
