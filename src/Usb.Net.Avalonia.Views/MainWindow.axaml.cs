using Usb.Net.Avalonia.Abstractions.ViewModels;
using Usb.Net.Avalonia.Views.Controls;

namespace Usb.Net.Avalonia.Views;

/// <summary>
/// Code-behind for the <see cref="MainWindow" />.
/// </summary>
internal sealed partial class MainWindow : WindowBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    /// <param name="locator"> An implementation of <see cref="IViewModelLocator" />. </param>
    public MainWindow(IViewModelLocator locator) : base(locator)
    {
        InitializeComponent();
    }
}
