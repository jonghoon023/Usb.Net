using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Usb.Net.Avalonia.Views.Services;

namespace Usb.Net.Avalonia.Views;

/// <summary>
/// Code-behind for the <see cref="App" /> class.
/// </summary>
/// <param name="logger"> An implementation of <see cref="ILogger{TCategoryName}" /> for logging purposes. </param>
/// <param name="provider"> An implementation of <see cref="IServiceProvider" />. </param>
internal sealed partial class App(ILogger<App> logger, IServiceProvider provider) : Application
{
    /// <inheritdoc cref="Application.Initialize" />
    public override void Initialize()
    {
        InitializeDataTemplates();
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc cref="Application.OnFrameworkInitializationCompleted" />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = provider.GetService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Registers the application's data templates using <see cref="ViewLocator" />.
    /// </summary>
    private void InitializeDataTemplates()
    {
        ViewLocator? viewLocator = provider.GetService<ViewLocator>();
        if (viewLocator != null)
        {
            DataTemplates.Add(viewLocator);
            LogDataTemplateRegistered(logger, viewLocator.GetType());
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Registered data template: {DataTemplateType}")]
    private static partial void LogDataTemplateRegistered(ILogger logger, Type dataTemplateType);
}