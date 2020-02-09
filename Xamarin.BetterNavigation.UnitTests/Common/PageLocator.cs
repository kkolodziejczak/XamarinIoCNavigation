using System;
using System.Collections.Generic;
using Xamarin.BetterNavigation.Forms;
using Xamarin.BetterNavigation.UnitTests.Common.Pages;
using Xamarin.BetterNavigation.UnitTests.Navigation;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Common
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
            { ApplicationPage.PageWithNavParameterPage.ToString(), typeof(PageWithNavParameterPage) },
        };

        public PageLocator(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public Page GetPage(string pageName)
        {
            return (Page)_serviceLocator.Get(PageMap[pageName]);
        }

        public string GetPageName(Page page)
        {
            var type = page.GetType();

            foreach (var (pageKey, pageType) in PageMap)
            {
                if(type == pageType)
                {
                    return pageKey;
                }
            }

            return default;
        }
    }
}
