using Usb.Net.Avalonia.ViewModels.Abstractions;

namespace Usb.Net.Avalonia.ViewModels.Messages
{
    /// <summary>
    /// Message class that sends a page navigation request.
    /// </summary>
    public sealed class NavigateRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateRequestMessage" /> class.
        /// </summary>
        /// <param name="viewModel"> The target page ViewModel to navigate to. </param>
        public NavigateRequestMessage(PageViewModelBase viewModel)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets the ViewModel representing the target page.
        /// </summary>
        public PageViewModelBase ViewModel { get; }
    }
}
