using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Usb.Net.Avalonia.Abstractions.ViewModels;
using Usb.Net.Avalonia.Abstractions.Views;
using Usb.Net.Avalonia.ViewModels.Abstractions;
using Usb.Net.Avalonia.ViewModels.Messages;
using Usb.Net.Avalonia.ViewModels.Pages;

namespace Usb.Net.Avalonia.ViewModels
{
    /// <summary>
    /// The ViewModel for the MainWindow.
    /// </summary>
    public sealed partial class MainWindowViewModel : WindowViewModelBase, IRecipient<NavigateRequestMessage>
    {
        private readonly IHostEnvironment _environment;
        private readonly INavigator _navigator;

        [ObservableProperty]
        private IViewModel? _pageContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="environment"> An implementation of <see cref="IHostEnvironment" />. </param>
        /// <param name="navigator"> An implementation of <see cref="INavigator" />. </param>
        /// <param name="window"> An implementation of <see cref="IWindow" />. </param>
        public MainWindowViewModel(IHostEnvironment environment, INavigator navigator, IWindow window) : base(window)
        {
            _environment = environment;
            _navigator = navigator;
        }

        /// <inheritdoc cref="ViewModelBase.OnInitialized" />
        public override void OnInitialized()
        {
            IsActive = true;
            Title = _environment.ApplicationName;
        }

        /// <inheritdoc cref="ViewModelBase.OnLoaded" />
        public override void OnLoaded()
        {
            _navigator.Push<MainPageViewModel>();
        }

        /// <inheritdoc cref="IRecipient{TMessage}.Receive(TMessage)" />
        public void Receive(NavigateRequestMessage message)
        {
            PageContext = message?.ViewModel;
        }
    }
}
