using System.Threading.Tasks;

namespace Usb.Net.Avalonia.Abstractions.ViewModels
{
    /// <summary>
    /// Interface that must be implemented by all ViewModel classes.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Called when the view is initialized.
        /// </summary>
        void OnInitialized();

        /// <summary>
        /// Called asynchronously when the view is initialized.
        /// </summary>
        Task OnInitializedAsync();

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        void OnLoaded();

        /// <summary>
        /// Called asynchronously when the view is loaded.
        /// </summary>
        Task OnLoadedAsync();

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        void OnUnloaded();

        /// <summary>
        /// Called asynchronously when the view is unloaded.
        /// </summary>
        Task OnUnloadedAsync();
    }
}
