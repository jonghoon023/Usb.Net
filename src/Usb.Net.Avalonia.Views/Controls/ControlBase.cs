using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Usb.Net.Avalonia.Abstractions.ViewModels;

namespace Usb.Net.Avalonia.Views.Controls;

/// <summary>
/// Abstract base class that must be inherited by all control UI components.
/// </summary>
internal abstract class ControlBase : UserControl
{
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
}
