using System;
using System.Collections.Generic;
using TestDI.Pages;
using Xamarin.BetterNavigation.Forms;
using Xamarin.Forms;

namespace TestDI.Common
{
    public class PageLocator : IPageLocator
    {
        private readonly IServiceLocator _serviceLocator;

        public static readonly Dictionary<string, Type> PageMap = new Dictionary<string, Type>
        {
            { ApplicationPage.MainMenuPage.ToString(), typeof(MainPage) },
            { ApplicationPage.SideBar.ToString(), typeof(StartPage) },
            { ApplicationPage.LoginPage.ToString(), typeof(LoginPage) },
            { ApplicationPage.ListViewPage.ToString(), typeof(ListViewPage) },
        };

        public PageLocator(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public Page GetPage(string pageName)
        {
            return (Page)_serviceLocator.Get(PageMap[pageName]);
        }

        public string GetPage(Page page)
        {
            foreach (var registeredPage in PageMap)
            {
                if (page.GetType().IsInstanceOfType(registeredPage.Value))
                {
                    return registeredPage.Key;
                }
            }

            return default;
        }
    }
}
