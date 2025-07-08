using CommunityToolkit.Mvvm.Input;
using Usb.Net.Avalonia.ViewModels.Abstractions;

namespace Usb.Net.Avalonia.ViewModels.Pages
{
    /// <summary>
    /// ViewModel for the MainPage.
    /// </summary>
    public sealed partial class MainPageViewModel : PageViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel" /> class.
        /// </summary>
        /// <param name="navigator"> An implementation of <see cref="INavigator" />. </param>
        public MainPageViewModel(INavigator navigator) : base(navigator)
        {
        }

        [RelayCommand]
        private void OnNavigateToUsbMonitorPage()
        {
            Navigator.Push<UsbMonitorPageViewModel>();
        }

        [RelayCommand]
        private void OnNavigateToUsbCommandPage()
        {
            Navigator.Push<UsbCommandPageViewModel>();
        }
    }
}
