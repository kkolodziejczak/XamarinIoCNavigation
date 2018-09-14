﻿using System.Linq;
using System.Threading.Tasks;
using TestDI.Interfaces;
using Xamarin.Forms;

namespace TestDI
{
    public static class NavigationPageOverride
    {
        public static Task GoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.GoToAsync(page.ToString(), navigationParameters);
        }

        public static void InsertPageAfter(this INavigation navigation, Page page, Page after)
        {
            var navigationPagelist = navigation.NavigationStack.ToList();
            var afterPageIndex = navigationPagelist.IndexOf(after);

            Page pageAboveAfter = null;
            if (afterPageIndex + 1 > navigation.NavigationStack.Count - 1)
            {
                pageAboveAfter = navigation.NavigationStack[afterPageIndex];
            }
            else
            {
                pageAboveAfter = navigation.NavigationStack[afterPageIndex + 1];
            }
            navigation.InsertPageBefore(page, pageAboveAfter);
        }
    }
}
