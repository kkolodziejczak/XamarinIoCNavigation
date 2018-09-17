using System;
using System.Collections.Generic;
using TestDI.Pages;

namespace TestDI.Navigation
{
    public static class NavigationPageMap
    {
        public static readonly Dictionary<string, Type> PageMap = new Dictionary<string, Type>
        {
            { ApplicationPage.MainMenuPage.ToString(), typeof(MainPage) },
            { ApplicationPage.SideBar.ToString(), typeof(StartPage) },
            { ApplicationPage.LoginPage.ToString(), typeof(LoginPage) },
            { ApplicationPage.ListViewPage.ToString(), typeof(ListViewPage) },
        };
    }
}
