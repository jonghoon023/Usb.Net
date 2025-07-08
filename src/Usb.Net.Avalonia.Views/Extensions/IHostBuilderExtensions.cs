using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Usb.Net.Avalonia.Abstractions.Views;

namespace Usb.Net.Avalonia.Views.Extensions;

/// <summary>
/// Static class that contains extension methods for <see cref="IHostBuilder" />.
/// </summary>
internal static partial class IHostBuilderExtensions
{
    private const string VersionPattern = "{Version}";

    /// <summary>
    /// Registers a file logger using Serilog to write logs to a file.
    /// </summary>
    /// <param name="hostBuilder"> An implementation of <see cref="IHostBuilder" />. </param>
    /// <returns> The same <see cref="IHostBuilder" /> instance after registering the file logger. </returns>
    public static IHostBuilder UseSerilogWithFile(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((hostingContext, provider, loggerConfiguration) =>
        {
            UpdateLogFilePath(provider, hostingContext.Configuration);
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .ReadFrom.Services(provider);
        });
    }

    /// <summary>
    /// Updates the log file path in the configuration to include version and full path.
    /// </summary>
    /// <param name="provider"> An implementation of <see cref="IServiceProvider" />. </param>
    /// <param name="configuration"> An implementation of <see cref="IConfiguration" />. </param>
    /// <seealso href="https://stackoverflow.com/a/75988212" />
    private static void UpdateLogFilePath(IServiceProvider provider, IConfiguration configuration)
    {
        IAppInfo appInfo = provider.GetRequiredService<IAppInfo>();
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (KeyValuePair<string, string?> keyValuePair in configuration.AsEnumerable())
        {
            if (LogFilePathRegex().IsMatch(keyValuePair.Key) && !string.IsNullOrEmpty(keyValuePair.Value))
            {
                string logFilePath = Path.Combine(appInfo.AppDataDirectory, configuration[keyValuePair.Key] ?? string.Empty);
                configuration[keyValuePair.Key] = logFilePath.Replace(VersionPattern, appInfo.Version.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
    }

    [GeneratedRegex("^Serilog:WriteTo.*Args:path$", RegexOptions.IgnoreCase)]
    private static partial Regex LogFilePathRegex();
}