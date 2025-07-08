using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Usb.Net.Avalonia.Abstractions.ViewModels;

namespace Usb.Net.Avalonia.Views.Services;

/// <summary>
/// An implementation of <see cref="IDataTemplate" /> that locates and returns the corresponding view for a ViewModel.
/// </summary>
/// <param name="logger"> An implementation of <see cref="ILogger{TCategoryName}" /> used for logging. </param>
/// <param name="provider"> An implementation of <see cref="IServiceProvider" />. </param>
internal sealed partial class ViewLocator(ILogger<ViewLocator> logger, IServiceProvider provider) : IDataTemplate
{
    private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private readonly ConcurrentDictionary<Type, Control> _controls = new();

    /// <inheritdoc cref="IDataTemplate.Match(object?)" />
    public bool Match(object? data)
    {
        return data is IViewModel;
    }

    /// <summary>
    /// Creates and returns a <see cref="Control" /> instance that corresponds to the given ViewModel.
    /// </summary>
    /// <param name="param"> The ViewModel instance. </param>
    /// <returns> A new or cached <see cref="Control" /> instance. </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="param" /> is <see langword="null" />. </exception>
    /// <exception cref="ArgumentException"> Thrown when a view could not be created for the given <paramref name="param" />. </exception>
    public Control Build(object? param)
    {
        Type viewModelType = param?.GetType() ?? throw new ArgumentNullException(nameof(param));

        if (_controls.TryGetValue(viewModelType, out Control? control))
        {
            LogViewRetrievedFromCache(logger, viewModelType.Name);
            return control;
        }
        else if (GetViewType(viewModelType) is Type viewType &&
                 ActivatorUtilities.CreateInstance(provider, viewType) is Control createdControl)
        {
            LogViewCreatedSuccessfully(logger, viewType.Name, viewModelType.Name);
            return _controls.GetOrAdd(viewModelType, createdControl);
        }

        LogViewCreationFailed(logger, viewModelType.Name);
        throw new ArgumentException($"Unable to create a {nameof(Control)} for ViewModel of type {viewModelType.Name}.", nameof(param));
    }

    private Type? GetViewType(Type viewModelType)
    {
        string viewName = viewModelType.Name.Replace("ViewModel", string.Empty, StringComparison.InvariantCulture);
        IEnumerable<Type> viewTypes = _assembly.GetTypes().Where(type => type.IsClass && type.Name == viewName);
        Type? resolvedType = viewTypes.SingleOrDefault();

        if (resolvedType == null)
        {
            LogViewTypeNotFound(logger, viewModelType.Name, viewName);
        }

        return resolvedType;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "View retrieved from cache for ViewModel: {ViewModelType}")]
    private static partial void LogViewRetrievedFromCache(ILogger logger, string viewModelType);

    [LoggerMessage(Level = LogLevel.Information, Message = "View '{ViewType}' successfully created for ViewModel '{ViewModelType}'")]
    private static partial void LogViewCreatedSuccessfully(ILogger logger, string viewType, string viewModelType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "View type not found for ViewModel: {ViewModelType}. Expected view name: '{ExpectedViewName}'")]
    private static partial void LogViewTypeNotFound(ILogger logger, string viewModelType, string expectedViewName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to create view for ViewModel: {ViewModelType}")]
    private static partial void LogViewCreationFailed(ILogger logger, string viewModelType);
}
