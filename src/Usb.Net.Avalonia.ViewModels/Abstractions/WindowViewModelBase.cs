using CommunityToolkit.Mvvm.ComponentModel;
using Usb.Net.Avalonia.Abstractions.ViewModels;
using Usb.Net.Avalonia.Abstractions.Views;

namespace Usb.Net.Avalonia.ViewModels.Abstractions
{
    /// <summary>
    /// An abstract implementation of <see cref="IWindowViewModel" /> for Window-based ViewModels.
    /// </summary>
    public abstract partial class WindowViewModelBase : ViewModelBase, IWindowViewModel
    {
        private readonly IWindow _window;

        [ObservableProperty]
        private string _title;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewModelBase" /> class.
        /// </summary>
        /// <param name="window"> An implementation of <see cref="IWindow" /> representing the actual Window UI object. </param>
        protected WindowViewModelBase(IWindow window)
        {
            _window = window;
            _title = string.Empty;

            IsActive = true;
        }

        /// <inheritdoc cref="IWindowViewModel.OnClosing" />
        public virtual bool OnClosing()
        {
            return false;
        }

        /// <inheritdoc cref="IWindowViewModel.OnClosed" />
        public virtual void OnClosed()
        {
            IsActive = false;
        }

        /// <summary>
        /// Opens the window.
        /// </summary>
        protected void Show()
        {
            _window.Show();
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        protected void Hide()
        {
            _window.Hide();
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        protected void Close()
        {
            _window.Close();
        }
    }
}
