namespace Usb.Net.Avalonia.Abstractions.ViewModels
{
    /// <summary>
    /// Interface that must be implemented by the ViewModel of a window UI object.
    /// </summary>
    public interface IWindowViewModel : IViewModel
    {
        /// <summary>
        /// Called when the window is about to close.
        /// </summary>
        /// <returns> Return <see langword="true" /> to cancel the window closing, or <see langword="false" /> to allow it. </returns>
        bool OnClosing();

        /// <summary>
        /// Called after the window has been closed.
        /// </summary>
        void OnClosed();
    }
}
