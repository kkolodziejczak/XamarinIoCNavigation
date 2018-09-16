using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestDI.Navigation;
using TestDI.Pages;
using Xamarin.Forms;

namespace TestDI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly INavigation _pageNavigation;
        private readonly IServiceLocator _serviceLocator;
        private readonly Dictionary<string, object> _naivagionParameters;
        private readonly Dictionary<ApplicationPage, Type> _applicationPages = new Dictionary<ApplicationPage, Type>
        {
            { ApplicationPage.MainMenuPage, typeof(MainPage) },
            { ApplicationPage.SideBar, typeof(StartPage) },
            { ApplicationPage.LoginPage, typeof(LoginPage) },
            { ApplicationPage.ListViewPage, typeof(ListViewPage) },
        };

        public NavigationService(INavigation navigation, IServiceLocator serviceLocator)
        {
            _naivagionParameters = new Dictionary<string, object>();
            _pageNavigation = navigation;
            _serviceLocator = serviceLocator;
        }

        public T NavigationParameters<T>(string parameterKey)
        {
            if (_naivagionParameters.ContainsKey(parameterKey))
            {
                if (_naivagionParameters[parameterKey] is T value)
                {
                    return value;
                }
                throw new InvalidCastException($"{nameof(parameterKey)} is not a type of {typeof(T)}.");
            }
            throw new KeyNotFoundException($"{nameof(parameterKey)} was not found in NavigationParameters");
        }

        public Task PopPageToRootAsync()
            => _pageNavigation.PopToRootAsync();

        public Task PopPageAsync()
            => PopPageAsync(1);

        public Task PopPageAsync(byte count)
            => RemoveUnwantedPages(count, null);

        public Task GoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters)
        {
            InitializeNavigationParameters(navigationParameters);
            return _pageNavigation.PushAsync(GetNewPage(destinationPageName));
        }

        public Task PopPageAndGoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(1, destinationPageName, navigationParameters);

        public Task PopPageAndGoToAsync(byte numberOfPagesToPop, string destinationPageName, params (string key, object value)[] navigationParameters)
            => RemoveUnwantedPages(numberOfPagesToPop, () => 
            {
                var lastPage = GetPage(GetLastPageIndex());
                var newPage = GetNewPage(destinationPageName);
                _pageNavigation.InsertPageBefore(newPage, lastPage);

                InitializeNavigationParameters(navigationParameters);
            });

        private Task RemoveUnwantedPages(byte count, Action actionBeforePop)
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

            return _pageNavigation.PopAsync();
        }

        private Page GetNewPage(string destinationPageName)
        {
            Enum.TryParse(destinationPageName, out ApplicationPage applicationPage);
            return (Page)_serviceLocator.Get(_applicationPages[applicationPage]);
        }

        private Page GetPage(int index)
            => _pageNavigation.NavigationStack[index];

        private int GetLastPageIndex()
            => _pageNavigation.NavigationStack.Count - 1; // -1 because we start counting from 0

        private void InitializeNavigationParameters(params (string key, object value)[] navigationParameters)
        {
            _naivagionParameters.Clear();
            foreach (var (key, value) in navigationParameters)
            {
                _naivagionParameters.Add(key, value);
            }
        }
    }
}
