using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using Usb.Net.Avalonia.Abstractions.ViewModels;

namespace Usb.Net.Avalonia.ViewModels.Abstractions
{
    /// <summary>
    /// An abstract implementation of the <see cref="IViewModel" /> interface.
    /// </summary>
    public abstract class ViewModelBase : ObservableRecipient, IViewModel
    {
        /// <inheritdoc cref="IViewModel.OnInitialized" />
        public virtual void OnInitialized()
        {
        }

        /// <inheritdoc cref="IViewModel.OnInitializedAsync" />
        public virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IViewModel.OnLoaded" />
        public virtual void OnLoaded()
        {
        }

        /// <inheritdoc cref="IViewModel.OnLoadedAsync" />
        public virtual Task OnLoadedAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IViewModel.OnUnloaded" />
        public virtual void OnUnloaded()
        {
        }

        /// <inheritdoc cref="IViewModel.OnUnloadedAsync" />
        public virtual Task OnUnloadedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
