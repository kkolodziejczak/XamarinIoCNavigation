using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="navigation"><see cref="INavigation"/> property from your <see cref="NavigationPage"/>.</param>
        /// <param name="pageLocator"><see cref="IPageLocator"/> that you created.</param>
        public NavigationService(INavigation navigation, IPageLocator pageLocator)
        {
            _navigationParameters = new Dictionary<string, object>();
            _pageNavigation = navigation;
            _pageLocator = pageLocator;
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
        /// Removes all pages from Navigation Stack.
        /// </summary>
        public Task PopPageToRootAsync()
            => PopPageToRootAsync(false);

        /// <summary>
        /// Removes all pages from Navigation Stack.
        /// </summary>
        /// <param name="animated">Animate the passage.</param>
        public Task PopPageToRootAsync(bool animated)
            => _pageNavigation.PopToRootAsync(animated);

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
        /// <exception cref="ArgumentOutOfRangeException"/>
        public Task PopPageAsync(byte amount)
            => PopPageAsync(amount, false);

        /// <summary>
        /// Removes <paramref name="amount"/> of pages from Navigation Stack.
        /// </summary>s
        /// <param name="amount">Number of pages to pop.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public Task PopPageAsync(byte amount, bool animated)
            => RemoveUnwantedPages(amount, null, animated);

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
            return _pageNavigation.PushAsync(_pageLocator.GetPage(pageName), animated);
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
                    _pageNavigation.RemovePage(pageToRemove);
                }
            }

            actionBeforePop?.Invoke();

            return _pageNavigation.PopAsync(animated);
        }

        private Page GetPage(int index)
            => _pageNavigation.NavigationStack[index];

        private int GetLastPageIndex()
            => _pageNavigation.NavigationStack.Count - 1; // -1 because we start amounting from 0

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
