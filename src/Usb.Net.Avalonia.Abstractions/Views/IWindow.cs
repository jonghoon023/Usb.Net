namespace Usb.Net.Avalonia.Abstractions.Views
{
    /// <summary>
    /// Interface that must be implemented by all Window classes.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Opens the window.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the window.
        /// </summary>
        void Hide();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
