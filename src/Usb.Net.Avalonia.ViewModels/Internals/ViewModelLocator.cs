using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Usb.Net.Avalonia.Abstractions.ViewModels;

namespace Usb.Net.Avalonia.ViewModels.Internals
{
    /// <summary>
    /// Implementation of <see cref="IViewModelLocator" />.
    /// </summary>
    internal sealed partial class ViewModelLocator : IViewModelLocator
    {
        private const string SuffixWord = "ViewModel";
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly Type[] _types = _assembly.GetTypes();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator" /> class.
        /// </summary>
        /// <param name="logger"> An instance of <see cref="ILogger{TCategoryName}" /> for logging purposes. </param>
        /// <param name="provider"> An instance of <see cref="IServiceProvider" />. </param>
        public ViewModelLocator(ILogger<ViewModelLocator> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        /// <inheritdoc cref="IViewModelLocator.GetViewModelFromViewType{TViewModel}(Type, object[])" />
        public TViewModel? GetViewModelFromViewType<TViewModel>(Type viewType, params object[] parameters)
            where TViewModel : class, IViewModel
        {
            if (viewType.Assembly == _assembly)
            {
                throw new ArgumentException($"{nameof(viewType)} must not be from the same assembly as {typeof(TViewModel)}.", nameof(viewType));
            }

            if (GetViewModelType(viewType) is Type viewModelType)
            {
                LogViewModelFromViewTypeResolved(_logger, viewType.FullName ?? viewType.Name, viewModelType.FullName);
                return GetViewModel<TViewModel>(viewModelType, parameters);
            }

            LogViewModelFromViewTypeNotResolved(_logger, viewType.FullName ?? viewType.Name);
            return default;
        }

        /// <inheritdoc cref="IViewModelLocator.GetViewModel{TViewModel}(Type, object[])" />
        public TViewModel? GetViewModel<TViewModel>(Type viewModelType, params object[] parameters)
            where TViewModel : class, IViewModel
        {
            if (_provider.GetService(viewModelType) is TViewModel viewModel)
            {
                LogResolvedFromDI(_logger, viewModelType.FullName ?? viewModelType.Name);
                return viewModel;
            }

            LogCreatedManually(_logger, viewModelType.FullName ?? viewModelType.Name);
            return ActivatorUtilities.CreateInstance(_provider, viewModelType, parameters) as TViewModel;
        }

        /// <inheritdoc cref="IViewModelLocator.GetViewModel{TViewModel}(string, object[])" />
        public TViewModel? GetViewModel<TViewModel>(string viewModelName, params object[] parameters)
            where TViewModel : class, IViewModel
        {
            if (GetViewModelType(viewModelName) is Type viewModelType)
            {
                LogViewModelByNameResolved(_logger, viewModelName, viewModelType.FullName);
                return GetViewModel<TViewModel>(viewModelType, parameters);
            }

            LogViewModelByNameNotResolved(_logger, viewModelName);
            return default;
        }

        /// <inheritdoc cref="IViewModelLocator.GetOrCreateViewModel{TViewModel}(object[])" />
        public TViewModel? GetOrCreateViewModel<TViewModel>(params object[] parameters)
            where TViewModel : class, IViewModel
        {
            LogViewModelGetOrCreateRequested(_logger, typeof(TViewModel).FullName ?? typeof(TViewModel).Name);
            return GetViewModel<TViewModel>(typeof(TViewModel), parameters);
        }

        private Type GetViewModelType(string viewModelName)
        {
            IEnumerable<Type> viewModelTypes = _types.Where(type => type.IsClass && type.Name == viewModelName);
            return viewModelTypes.Single();
        }

        private Type? GetViewModelType(Type viewType)
        {
            return GetViewModelType(viewType.Name + SuffixWord);
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "View type: {ViewType}, resolved ViewModel type: {ResolvedViewModelType}")]
        private static partial void LogViewModelFromViewTypeResolved(ILogger logger, string viewType, string? resolvedViewModelType);

        [LoggerMessage(Level = LogLevel.Warning, Message = "View type: {ViewType} could not be mapped to any ViewModel type.")]
        private static partial void LogViewModelFromViewTypeNotResolved(ILogger logger, string viewType);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Resolved via DI container: {ViewModelType}")]
        private static partial void LogResolvedFromDI(ILogger logger, string viewModelType);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Created manually using ActivatorUtilities: {ViewModelType}")]
        private static partial void LogCreatedManually(ILogger logger, string viewModelType);

        [LoggerMessage(Level = LogLevel.Debug, Message = "ViewModel name: {ViewModelName}, resolved ViewModel type: {ResolvedViewModelType}")]
        private static partial void LogViewModelByNameResolved(ILogger logger, string viewModelName, string? resolvedViewModelType);

        [LoggerMessage(Level = LogLevel.Warning, Message = "ViewModel with name '{ViewModelName}' could not be resolved.")]
        private static partial void LogViewModelByNameNotResolved(ILogger logger, string viewModelName);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Requested ViewModel type: {ViewModelType}")]
        private static partial void LogViewModelGetOrCreateRequested(ILogger logger, string viewModelType);
    }
}
