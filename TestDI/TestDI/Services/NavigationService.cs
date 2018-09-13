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
        };

        public NavigationService(INavigation navigation, IServiceLocalisator serviceLocalisator)
        {
            _naivagionParameters = new Dictionary<string, object>();
            _pageNavigation = navigation;
            _serviceLocalisator = serviceLocalisator;
        }

        /// <exception cref="KeyNotFoundException"/>
        public T NavigationParameters<T>(string parameterKey)
        {
            if (_naivagionParameters.ContainsKey(parameterKey))
            {
                return (T)_naivagionParameters[parameterKey];
            }
            throw new KeyNotFoundException();
        }

        public Task PopPageToRootAsync()
        {
            return _pageNavigation.PopToRootAsync();
        }

        public Task PopPageAsync(byte pagesToPop = 1)
        {
            var currentPageStackSize = _pageNavigation.NavigationStack.Count - 1; // -1 because we start counting from 0

            if (pagesToPop > currentPageStackSize)
            {
                throw new IndexOutOfRangeException("You want to pop too many pages. That there is on the Navigation Stack.");
            }

            if (pagesToPop >= 2)
            {
                for(var i = 1; i <= pagesToPop -1; i++) // -1 because we always pop minimum once at the end
                {
                    var pageToRemove = _pageNavigation.NavigationStack[currentPageStackSize - i];
                    _pageNavigation.RemovePage(pageToRemove);
                }
            }

            return _pageNavigation.PopAsync();
        }

        public Task GoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters)
        {
            Enum.TryParse(destinationPageName, out ApplicationPage applicationPage);
            var destinationPage = _serviceLocalisator.Get(_applicationPages[applicationPage]);

            _naivagionParameters.Clear();
            foreach (var (key, value) in navigationParameters)
            {
                _naivagionParameters.Add(key, value);
            }

            return _pageNavigation.PushAsync((Page)destinationPage);
        }

    }
}
