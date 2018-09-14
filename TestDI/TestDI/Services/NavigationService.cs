using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestDI.Interfaces;
using TestDI.Pages;
using Xamarin.Forms;

namespace TestDI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly INavigation _pageNavigation;
        private readonly IServiceLocalisator _serviceLocalisator;
        private readonly Dictionary<string, object> _naivagionParameters;
        private readonly Dictionary<ApplicationPage, Type> _applicationPages = new Dictionary<ApplicationPage, Type>
        {
            { ApplicationPage.LoginPage, typeof(LoginPage) },
            { ApplicationPage.MainMenuPage, typeof(MainPage) },
            { ApplicationPage.SideBar, typeof(StartPage) },
            { ApplicationPage.ListViewPage, typeof(ListViewPage) },
        };

        public NavigationService(INavigation navigation, IServiceLocalisator serviceLocalisator)
        {
            _naivagionParameters = new Dictionary<string, object>();
            _pageNavigation = navigation;
            _serviceLocalisator = serviceLocalisator;
        }

        public T NavigationParameters<T>(string parameterKey)
        {
            if (_naivagionParameters.ContainsKey(parameterKey))
            {
                return (T)_naivagionParameters[parameterKey];
            }
            throw new KeyNotFoundException();
        }

        public Task PopPageToRootAsync()
            => _pageNavigation.PopToRootAsync();

        public Task PopPageAsync()
            => PopPageAsync(1);

        public Task PopPageAsync(byte count)
        {
            var lastPageIndex = GetLastPageIndex();

            if (count > lastPageIndex)
            {
                throw new IndexOutOfRangeException("You want to remove too many pages from Navigation Stack.");
            }

            if (count >= 2)
            {
                for (var i = 1; i <= count - 1; i++) // -1 because we always pop minimum once at the end
                {
                    var pageToRemove = GetPage(lastPageIndex - i);
                    _pageNavigation.RemovePage(pageToRemove);
                }
            }

            return _pageNavigation.PopAsync();
        }

        public Task GoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters)
        {
            InitializeNavigationParameters(navigationParameters);
            return _pageNavigation.PushAsync(GetNewPage(destinationPageName));
        }

        public Task PopPageAndGoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters)
            => PopPageAndGoToAsync(1, destinationPageName, navigationParameters);

        public Task PopPageAndGoToAsync(byte numberOfPagesToPop, string destinationPageName, params (string key, object value)[] navigationParameters)
        {
            var lastPageIndex = GetLastPageIndex();

            if (numberOfPagesToPop > lastPageIndex + 1)
            {
                throw new IndexOutOfRangeException("You want to pop too many pages. That there is on the Navigation Stack.");
            }

            // remove unwanted pages
            for (var i = 1; i <= numberOfPagesToPop - 1; i++) // -1 because we always pop minimum once at the end
            {
                var pageToRemove = GetPage(lastPageIndex - i);
                _pageNavigation.RemovePage(pageToRemove);
            }

            var pageIndex = lastPageIndex == 0 ? lastPageIndex : lastPageIndex - numberOfPagesToPop;
            var pageAfterPopping = GetPage(pageIndex);
            var newPage = GetNewPage(destinationPageName);
            _pageNavigation.InsertPageAfter(newPage, pageAfterPopping);

            InitializeNavigationParameters(navigationParameters);

            return PopPageAsync();
        }

        private Page GetNewPage(string destinationPageName)
        {
            Enum.TryParse(destinationPageName, out ApplicationPage applicationPage);
            return (Page)_serviceLocalisator.Get(_applicationPages[applicationPage]);
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
