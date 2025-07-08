using System;

namespace Usb.Net.Avalonia.Abstractions.ViewModels
{
    /// <summary>
    /// Locator interface for managing ViewModels.
    /// </summary>
    public interface IViewModelLocator
    {
        /// <summary>
        /// Gets an instance of <typeparamref name="TViewModel" /> based on the view type.
        /// </summary>
        /// <typeparam name="TViewModel"> A class that implements the <see cref="IViewModel" /> interface. </typeparam>
        /// <param name="viewType"> The type of the view. </param>
        /// <param name="parameters"> Parameters required to create the ViewModel instance. </param>
        /// <returns> Returns an instance of <typeparamref name="TViewModel" /> if it can be created; otherwise, returns <see langword="null" />. </returns>
        /// <exception cref="ArgumentException"> Thrown when the assembly of <paramref name="viewType" /> is the same as the assembly of <typeparamref name="TViewModel" />. </exception>
        TViewModel? GetViewModelFromViewType<TViewModel>(Type viewType, params object[] parameters)
            where TViewModel : class, IViewModel;

        /// <summary>
        /// Gets an instance of <typeparamref name="TViewModel" /> based on the ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel"> A class that implements the <see cref="IViewModel" /> interface. </typeparam>
        /// <param name="viewModelType"> The type of the ViewModel. </param>
        /// <param name="parameters"> Parameters required to create the ViewModel instance. </param>
        /// <returns> Returns an instance of <typeparamref name="TViewModel" /> if it can be created; otherwise, returns <see langword="null" />. </returns>
        TViewModel? GetViewModel<TViewModel>(Type viewModelType, params object[] parameters)
            where TViewModel : class, IViewModel;

        /// <summary>
        /// Gets an instance of <typeparamref name="TViewModel" /> based on the ViewModel name.
        /// </summary>
        /// <typeparam name="TViewModel"> A class that implements the <see cref="IViewModel" /> interface. </typeparam>
        /// <param name="viewModelName"> The name of the ViewModel. </param>
        /// <param name="parameters"> Parameters required to create the ViewModel instance. </param>
        /// <returns> Returns an instance of <typeparamref name="TViewModel" /> if it can be created; otherwise, returns <see langword="null" />. </returns>
        TViewModel? GetViewModel<TViewModel>(string viewModelName, params object[] parameters)
            where TViewModel : class, IViewModel;

        /// <summary>
        /// Gets or creates an instance of <typeparamref name="TViewModel" />.
        /// </summary>
        /// <typeparam name="TViewModel"> A class that implements the <see cref="IViewModel" /> interface. </typeparam>
        /// <param name="parameters"> Parameters required to create the ViewModel instance. </param>
        /// <returns> Returns an instance of <typeparamref name="TViewModel" /> if it can be created; otherwise, returns <see langword="null" />. </returns>
        TViewModel? GetOrCreateViewModel<TViewModel>(params object[] parameters)
            where TViewModel : class, IViewModel;
    }
}
