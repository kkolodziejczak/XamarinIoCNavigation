using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.BetterNavigation.Core;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{
    public class NavigationService : INavigationService
    {
        private readonly INavigation _pageNavigation;
        private readonly IPageLocator _pageLocator;
        private readonly Dictionary<string, object> _navigationParameters;

        public NavigationService(INavigation navigation, IPageLocator pageLocator)
        {
            _navigationParameters = new Dictionary<string, object>();
            _pageNavigation = navigation;
            _pageLocator = pageLocator;
        }

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

        public Task PopPageToRootAsync()
            => PopPageToRootAsync(false);

        public Task PopPageToRootAsync(bool animated)
            => _pageNavigation.PopToRootAsync(animated);

        public Task PopPageAsync()
            => PopPageAsync(1, false);

        public Task PopPageAsync(bool animated)
            => PopPageAsync(1, animated);

        public Task PopPageAsync(byte count)
            => PopPageAsync(count, false);

        public Task PopPageAsync(byte count, bool animated)
            => RemoveUnwantedPages(count, null, animated);

        public Task GoToAsync(string pageName, params (string key, object value)[] navigationParameters)
            => GoToAsync(pageName, false, navigationParameters);

        public Task GoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters)
        {
            InitializeNavigationParameters(navigationParameters);
            return _pageNavigation.PushAsync(_pageLocator.GetPage(pageName), animated);
        }

        public Task PopPageAndGoToAsync(string pageName, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(1, pageName, false, navigationParameters);

        public Task PopPageAndGoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(1, pageName, animated, navigationParameters);

        public Task PopPageAndGoToAsync(byte amount, string pageName, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(amount, pageName, false, navigationParameters);

        public Task PopPageAndGoToAsync(byte amount, string pageName, bool animated, params (string key, object value)[] navigationParameters)
            => RemoveUnwantedPages(amount, () =>
            {
                var lastPage = GetPage(GetLastPageIndex());
                var newPage = _pageLocator.GetPage(pageName);
                _pageNavigation.InsertPageBefore(newPage, lastPage);

                InitializeNavigationParameters(navigationParameters);
            }, animated);

        private Task RemoveUnwantedPages(byte count, Action actionBeforePop, bool animated)
        {
            var lastPageIndex = GetLastPageIndex();
            var weWantToPopOnlyFirstPage = count == 1 && lastPageIndex == 0;

            if (count > lastPageIndex && !weWantToPopOnlyFirstPage)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "You want to remove too many pages from Navigation Stack.");
            }

            if (count >= 2)
            {
                for (var i = 1; i <= count - 1; i++) // -1 because we always pop minimum once at the end
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
            => _pageNavigation.NavigationStack.Count - 1; // -1 because we start counting from 0

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
