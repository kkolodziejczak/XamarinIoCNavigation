using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.BetterNavigation.Core;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{
    /// <summary>
    /// <see cref="NavigationService"/> provides navigation through your application.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly INavigation _pageNavigation;
        private readonly IPageLocator _pageLocator;
        private readonly Dictionary<string, object> _navigationParameters;
        private readonly Action<Page> _externalActionBeforePush;
        private readonly Action<Page> _externalActionBeforePop;

        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="navigation"><see cref="INavigation"/> property from your <see cref="NavigationPage"/>.</param>
        /// <param name="pageLocator"><see cref="IPageLocator"/> that you created.</param>
        public NavigationService(INavigation navigation, IPageLocator pageLocator)
            : this(navigation, pageLocator, null, null) { }


        /// <summary>
        /// Constructor with actions before pop and push
        /// </summary>
        /// <param name="navigation"><see cref="INavigation"/> property from your <see cref="NavigationPage"/>.</param>
        /// <param name="pageLocator"><see cref="IPageLocator"/> that you created.</param>
        /// <param name="actionBeforePop">
        /// Action that will be executed every time before <see cref="Page"/> will be taken from navigation stack.
        /// Passing <see cref="Page"/> object that will be removed.
        /// </param>
        /// <param name="actionBeforePush">
        /// Action that will be executed every time before <see cref="Page"/> will be pushed on top of the navigation stack.
        /// Passing <see cref="Page"/> object that will be removed.
        /// </param>
        public NavigationService(INavigation navigation, IPageLocator pageLocator, Action<Page> actionBeforePop, Action<Page> actionBeforePush)
        {
            _navigationParameters = new Dictionary<string, object>();
            _pageNavigation = navigation;
            _pageLocator = pageLocator;
            _externalActionBeforePop = actionBeforePop;
            _externalActionBeforePush = actionBeforePush;
        }

        /// <summary>
        /// Navigation parameters passed while navigating to this page.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to get.</typeparam>
        /// <param name="parameterKey">Key passed while navigating to this page.</param>
        /// <exception cref="KeyNotFoundException">Thrown when key is not present in <see cref="NavigationParameters{T}(string)"/></exception>
        /// <exception cref="InvalidCastException">Thrown when object type is not the same as requested one.</exception>
        public T NavigationParameters<T>(string parameterKey)
        {
            if (_navigationParameters.ContainsKey(parameterKey))
            {
                if (_navigationParameters[parameterKey] is T value)
                {
                    return value;
                }
                throw new InvalidCastException($"{nameof(parameterKey)} is not a type of {typeof(T)}.");
            }
            throw new KeyNotFoundException($"{nameof(parameterKey)} was not found in NavigationParameters");
        }

        /// <summary>
        /// Determines whether the <see cref="NavigationParameters{T}"/> contains the specified key.
        /// </summary>
        /// <param name="parameterKey">The key to locate in the <see cref="NavigationParameters{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterKey"/> is null.</exception>
        /// <returns>true if the <see cref="NavigationParameters{T}"/> contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsParameterKey(string parameterKey)
            => _navigationParameters.ContainsKey(parameterKey);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to get.</typeparam>
        /// <param name="parameterKey">Key passed while navigating to this page.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Thrown when object type is not the same as requested one.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameterKey"/> is null.</exception>
        public bool TryGetValue<T>(string parameterKey, out T value)
        {
            var result = _navigationParameters.TryGetValue(parameterKey, out var commonValue);

            if (result == true)
            {
                if (commonValue is T returnValue)
                {
                    value = returnValue;
                }
                else
                {
                    throw new InvalidCastException($"{nameof(parameterKey)} is not a type of {typeof(T)}.");
                }
            }

            value = default;
            return result;
        }

        /// <summary>
        /// Removes all pages from Navigation Stack.
        /// </summary>
        public Task PopPageToRootAsync()
            => PopPageToRootAsync(false);

        /// <summary>
        /// Removes all pages from Navigation Stack.
        /// </summary>
        /// <param name="animated">Animate the passage.</param>
        public Task PopPageToRootAsync(bool animated)
            => RemoveUnwantedPages((byte) GetLastPageIndex(), null, animated);

        /// <summary>
        /// Removes current page from Navigation Stack.
        /// </summary>
        public Task PopPageAsync()
            => PopPageAsync(1, false);

        /// <summary>
        /// Removes current page from Navigation Stack.
        /// </summary>
        /// <param name="animated">Animate the passage.</param>
        public Task PopPageAsync(bool animated)
            => PopPageAsync(1, animated);

        /// <summary>
        /// Removes <paramref name="amount"/> of pages from Navigation Stack.
        /// </summary>s
        /// <param name="amount">Number of pages to pop.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when you want to remove too many pages from the Navigation Stack.</exception>
        public Task PopPageAsync(byte amount)
            => PopPageAsync(amount, false);

        /// <summary>
        /// Removes <paramref name="amount"/> of pages from Navigation Stack.
        /// </summary>s
        /// <param name="amount">Number of pages to pop.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when you want to remove too many pages from the Navigation Stack.</exception>
        public Task PopPageAsync(byte amount, bool animated)
            => RemoveUnwantedPages(amount, null, animated);

        /// <summary>
        /// Removes all pages from Navigation Stack and navigates to <paramref name="pageName"/> <see cref="Page"/>.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when you want to remove too many pages from the Navigation Stack.</exception>
        public Task PopAllPagesAndGoToAsync(string pageName, params (string key, object value)[] navigationParameters)
            => PopAllPagesAndGoToAsync(pageName, false, navigationParameters);

        /// <summary>
        /// Removes all pages from Navigation Stack and navigates to <paramref name="pageName"/> <see cref="Page"/>.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when you want to remove too many pages from the Navigation Stack.</exception>
        public Task PopAllPagesAndGoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters)
            => RemoveUnwantedPages((byte)GetLastPageIndex(), () =>
            {
                // Remove 1st Page from the stack (root page)
                var firstPageOnTheStack = GetPage(GetLastPageIndex()-1);
                _externalActionBeforePop?.Invoke(firstPageOnTheStack);
                _pageNavigation.RemovePage(firstPageOnTheStack);

                var newPage = _pageLocator.GetPage(pageName);
                _externalActionBeforePush?.Invoke(newPage);
                _pageNavigation.InsertPageBefore(newPage, GetPage(GetLastPageIndex()));

                InitializeNavigationParameters(navigationParameters);
            }, animated);

        /// <summary>
        /// Navigate to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        public Task GoToAsync(string pageName, params (string key, object value)[] navigationParameters)
            => GoToAsync(pageName, false, navigationParameters);

        /// <summary>
        /// Navigate to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        public Task GoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters)
        {
            InitializeNavigationParameters(navigationParameters);
            var pageToPush = _pageLocator.GetPage(pageName);
            _externalActionBeforePush?.Invoke(pageToPush);
            return _pageNavigation.PushAsync(pageToPush, animated);
        }

        /// <summary>
        /// Pop current page and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        public Task PopPageAndGoToAsync(string pageName, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(1, pageName, false, navigationParameters);

        /// <summary>
        /// Pop current page and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        public Task PopPageAndGoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(1, pageName, animated, navigationParameters);

        /// <summary>
        /// Pop <paramref name="amount"/> of pages and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="amount">The amount of pages to pop.</param>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when you want to remove too many pages from the Navigation Stack.</exception>
        public Task PopPageAndGoToAsync(byte amount, string pageName, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(amount, pageName, false, navigationParameters);

        /// <summary>
        /// Pop <paramref name="amount"/> of pages and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="amount">The amount of pages to pop.</param>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when you want to remove too many pages from the Navigation Stack.</exception>
        public Task PopPageAndGoToAsync(byte amount, string pageName, bool animated, params (string key, object value)[] navigationParameters)
            => RemoveUnwantedPages(amount, () =>
            {
                var lastPage = GetPage(GetLastPageIndex());
                var newPage = _pageLocator.GetPage(pageName);
                _externalActionBeforePush?.Invoke(newPage);
                _pageNavigation.InsertPageBefore(newPage, lastPage);

                InitializeNavigationParameters(navigationParameters);
            }, animated);

        private Task RemoveUnwantedPages(byte amount, Action actionBeforePop, bool animated)
        {
            var lastPageIndex = GetLastPageIndex();
            var weWantToPopOnlyFirstPage = amount == 1 && lastPageIndex == 0;

            if (amount > lastPageIndex && !weWantToPopOnlyFirstPage)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "You want to remove too many pages from the Navigation Stack.");
            }

            if (amount >= 2)
            {
                for (var i = 1; i <= amount - 1; i++) // -1 because we always pop minimum once at the end
                {
                    var pageToRemove = GetPage(lastPageIndex - i);
                    _externalActionBeforePop?.Invoke(pageToRemove);
                    _pageNavigation.RemovePage(pageToRemove);
                }
            }

            actionBeforePop?.Invoke();
            _externalActionBeforePop?.Invoke(GetLastPage());
            return _pageNavigation.PopAsync(animated);
        }

        private Page GetPage(int index)
            => _pageNavigation.NavigationStack[index];

        private int GetLastPageIndex()
            => _pageNavigation.NavigationStack.Count - 1; // -1 because we start amounting from 0

        private Page GetLastPage()
            => _pageNavigation.NavigationStack.Last();

        private void InitializeNavigationParameters(params (string key, object value)[] navigationParameters)
        {
            _navigationParameters.Clear();
            foreach (var (key, value) in navigationParameters)
            {
                _navigationParameters.Add(key, value);
            }
        }
    }
}
