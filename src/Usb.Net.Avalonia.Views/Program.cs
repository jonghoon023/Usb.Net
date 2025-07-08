using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Serilog;
using System;
using Usb.Net.Avalonia.ViewModels.Extensions;
using Usb.Net.Avalonia.Views.Extensions;
using Usb.Net.Extensions;

namespace Usb.Net.Avalonia.Views;

/// <summary>
/// Static class that serves as the entry point of the application.
/// </summary>
internal static class Program
{
    private const string ApplicationNameSectionName = "Application:Name";
    private static IHost? _host;

    /// <summary>
    /// Application entry point.
    /// Do not use any Avalonia, third-party APIs, or code that depends on a SynchronizationContext before AppMain is called;
    /// the runtime is not fully initialized and may fail.
    /// </summary>
    /// <param name="args"> Command-line arguments. </param>
    [STAThread]
    public static void Main(string[] args)
    {
        _host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.SetBasePath(AppContext.BaseDirectory);
                builder.AddEnvironmentVariables();
                builder.AddAppSettings();
            })
            .ConfigureServices((context, services) =>
            {
                context.HostingEnvironment.ApplicationName = GetApplicationName(context.Configuration, context.HostingEnvironment);

                services.UseViewModel();
                services.AddSingletonServices();

                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();

                services.AddSingletonServices();
                services.AddOperatingSystemServices();
                services.AddUsb();
            })
            .ConfigureLogging(builder => builder.ClearProviders())
            .UseSerilogWithFile()
            .Build();

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "An extremely critical issue occurred while running the application.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>
    /// Avalonia configuration. Do not remove; also used by the visual designer.
    /// </summary>
    /// <returns> Returns an <see cref="AppBuilder" /> instance. </returns>
    /// <exception cref="ArgumentException"> Thrown when <see cref="_host" /> is not properly initialized. </exception>
    public static AppBuilder BuildAvaloniaApp()
    {
        if (_host != null)
        {
            IconProvider.Current.Register<FontAwesomeIconProvider>();
            return AppBuilder.Configure(_host.Services.GetRequiredService<App>)
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
        }

        throw new ArgumentException("Cannot start the application because the host is not initialized.");
    }

    private static string GetApplicationName(IConfiguration configuration, IHostEnvironment environment)
    {
        return configuration.GetValue<string>(ApplicationNameSectionName) ?? environment.ApplicationName;
    }
}