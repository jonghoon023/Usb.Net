using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Usb.Net.Avalonia.Abstractions.Views;
using Windows.Storage;

namespace Usb.Net.Avalonia.Views.Services;

/// <summary>
/// A static helper class that contains information about the <c>Windows</c> environment.
/// </summary>
internal sealed class WindowsAppInfo : IAppInfo
{
    private readonly string _appName;
    private readonly Assembly _assembly;

    private bool? _isPackaged;
    private Version? _version;
    private string _cacheDirectory;
    private string _appDataDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsAppInfo" /> class.
    /// </summary>
    /// <param name="environment"> An instance of <see cref="IHostEnvironment" />. </param>
    public WindowsAppInfo(IHostEnvironment environment)
    {
        _appName = environment.ApplicationName;
        _assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        _cacheDirectory = _appDataDirectory = string.Empty;
    }

    /// <inheritdoc cref="IAppInfo.IsPackaged" />
    public bool IsPackaged
    {
        get
        {
            if (!_isPackaged.HasValue)
            {
                _isPackaged = false;

#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    _isPackaged = ApplicationData.Current != null;
                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine(e, "This API is only available in packaged applications.");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e, "An unexpected error occurred while trying to retrieve the Application Data directory.");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return _isPackaged.Value;
        }
    }

    /// <inheritdoc cref="IAppInfo.Version" />
    public Version Version
    {
        get
        {
            if (_version == null)
            {
                _version = _assembly.GetName().Version ?? new Version(0, 0, 0, 0);
            }

            return _version;
        }
    }

    /// <inheritdoc cref="IAppInfo.CacheDirectory" />
    public string CacheDirectory
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_cacheDirectory))
            {
                _cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _appName, "Cache");
            }

            return _cacheDirectory;
        }
    }

    /// <inheritdoc cref="IAppInfo.AppDataDirectory" />
    public string AppDataDirectory
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_appDataDirectory))
            {
                _appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _appName, "Data");
            }

            return _appDataDirectory;
        }
    }
}