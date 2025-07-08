namespace Usb.Net.Avalonia.ViewModels.Abstractions
{
    /// <summary>
    /// Navigation service that helps navigate between pages.
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Removes the current page from the navigation history and navigates back to the previous page.
        /// </summary>
        void Pop();

        /// <summary>
        /// Navigates to a new page and adds it to the navigation history.
        /// </summary>
        /// <typeparam name="TViewModel"> A class that inherits from <see cref="PageViewModelBase" />. </typeparam>
        /// <param name="arguments"> Parameters required to create the <typeparamref name="TViewModel" /> instance. </param>
        void Push<TViewModel>(params object[] arguments)
            where TViewModel : PageViewModelBase;

        /// <summary>
        /// Navigates to a new page without adding it to the navigation history.
        /// </summary>
        /// <typeparam name="TViewModel"> A class that inherits from <see cref="PageViewModelBase" />. </typeparam>
        /// <param name="arguments"> Parameters required to create the <typeparamref name="TViewModel" /> instance. </param>
        void Replace<TViewModel>(params object[] arguments)
            where TViewModel : PageViewModelBase;
    }
}
