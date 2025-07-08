using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Usb.Net.Avalonia.Abstractions.ViewModels;
using Usb.Net.Avalonia.ViewModels.Abstractions;
using Usb.Net.Avalonia.ViewModels.Messages;

namespace Usb.Net.Avalonia.ViewModels.Internals
{
    /// <summary>
    /// Implementation of the <see cref="INavigator" /> interface.
    /// </summary>
    internal sealed partial class Navigator : INavigator
    {
        private const string ItemSeparator = ", ";

        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly IViewModelLocator _viewModelLocator;
        private readonly ConcurrentStack<PageViewModelBase> _navigationStack;

        private bool _isCurrentPageReplaced;

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigator" /> class.
        /// </summary>
        /// <param name="logger"> An instance of <see cref="ILogger{TCategoryName}" /> for logging purposes. </param>
        /// <param name="messenger"> An implementation of <see cref="IMessenger" />. </param>
        /// <param name="viewModelLocator"> An implementation of <see cref="IViewModelLocator" />. </param>
        public Navigator(ILogger<Navigator> logger, IMessenger messenger, IViewModelLocator viewModelLocator)
        {
            _logger = logger;
            _messenger = messenger;
            _viewModelLocator = viewModelLocator;
            _navigationStack = new ConcurrentStack<PageViewModelBase>();
        }

        /// <inheritdoc cref="INavigator.Pop" />
        public void Pop()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.TryPop(out _);
            }

            if (_navigationStack.TryPeek(out PageViewModelBase? pageViewModel))
            {
                Navigate(pageViewModel);
            }
        }

        /// <inheritdoc cref="INavigator.Push{TViewModel}(object[])" />
        public void Push<TViewModel>(params object[] arguments) where TViewModel : PageViewModelBase
        {
            NavigateTo(typeof(TViewModel), true, arguments);
        }

        /// <inheritdoc cref="INavigator.Replace{TViewModel}(object[])" />
        public void Replace<TViewModel>(params object[] arguments) where TViewModel : PageViewModelBase
        {
            NavigateTo(typeof(TViewModel), false, arguments);
        }

        private void NavigateTo(Type viewModelType, bool isPushToStack, params object[] arguments)
        {
            object[] args = arguments ?? Array.Empty<object>();
            PageViewModelBase? pageViewModel = _viewModelLocator.GetViewModel<PageViewModelBase>(viewModelType, args);

            if (pageViewModel != null)
            {
                _navigationStack.Push(pageViewModel);

                _isCurrentPageReplaced = !isPushToStack;
                if (_isCurrentPageReplaced)
                {
                    _navigationStack.TryPop(out _);
                }

                Navigate(pageViewModel);
                LogNavigationSucceeded(_logger, viewModelType.Name, string.Join(ItemSeparator, args.Select(arg => arg.ToString())));
            }
        }

        private void Navigate(PageViewModelBase viewModel)
        {
            _messenger.Send(new NavigateRequestMessage(viewModel));
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Successfully navigated to {ViewModelName}. Arguments: [{ArgumentsList}]")]
        private static partial void LogNavigationSucceeded(ILogger logger, string viewModelName, string argumentsList);
    }
}
