namespace Usb.Net.Avalonia.ViewModels.Abstractions
{
    /// <summary>
    /// Abstract base class that must be inherited by all page ViewModel classes.
    /// </summary>
    public abstract class PageViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageViewModelBase" /> class.
        /// </summary>
        /// <param name="navigator"> An implementation of <see cref="INavigator" /> used for page navigation. </param>
        protected PageViewModelBase(INavigator navigator)
        {
            Navigator = navigator;
        }

        /// <summary>
        /// Gets the implementation of <see cref="INavigator" /> that provides navigation functionality.
        /// </summary>
        public INavigator Navigator { get; }
    }
}
