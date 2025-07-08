using Microsoft.Extensions.Configuration;

namespace Usb.Net.Avalonia.Views.Extensions;

/// <summary>
/// Static class that contains extension methods for <see cref="IConfigurationBuilder" />.
/// </summary>
internal static class IConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds the <c> appsettings.json </c> file to the configuration.
    /// </summary>
    /// <param name="configurationBuilder"> An implementation of <see cref="IConfigurationBuilder" />. </param>
    /// <returns> The <see cref="IConfigurationBuilder" /> instance with the <c>appsettings.json</c> file added. </returns>
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder configurationBuilder)
    {
        return configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }
}
