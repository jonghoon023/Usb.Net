using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using Usb.Net.Avalonia.Abstractions.ViewModels;
using Usb.Net.Avalonia.Abstractions.Views;

namespace Usb.Net.Avalonia.Views.Controls;

/// <summary>
/// Abstract base class that must be inherited by all <see cref="Window" /> instances.
/// </summary>
internal abstract class WindowBase : Window, IWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowBase" /> class.
    /// </summary>
    /// <param name="locator"> An implementation of <see cref="IViewModelLocator" />. </param>
    protected WindowBase(IViewModelLocator locator)
    {
        ArgumentNullException.ThrowIfNull(locator);
        DataContext = locator.GetViewModelFromViewType<IWindowViewModel>(GetType(), this);
    }

    /// <inheritdoc cref="StyledElement.OnInitialized" />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (DataContext is IViewModel viewModel)
        {
            viewModel.OnInitialized();
            Dispatcher.UIThread.Invoke(viewModel.OnInitializedAsync);
        }
    }

    /// <inheritdoc cref="Control.OnLoaded(RoutedEventArgs)" />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IViewModel viewModel)
        {
            viewModel.OnLoaded();
            Dispatcher.UIThread.Invoke(viewModel.OnLoadedAsync);
        }
    }

    /// <inheritdoc cref="Control.OnUnloaded(RoutedEventArgs)" />
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (DataContext is IViewModel viewModel)
        {
            viewModel.OnUnloaded();
            Dispatcher.UIThread.Invoke(viewModel.OnUnloadedAsync);
        }
    }

    /// <inheritdoc cref="TopLevel.OnClosed(EventArgs)" />
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        (DataContext as IWindowViewModel)?.OnClosed();
    }

    /// <inheritdoc cref="Window.OnClosing(WindowClosingEventArgs)" />
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        e.Cancel = (DataContext as IWindowViewModel)?.OnClosing() ?? false;
        base.OnClosing(e);
    }
}
