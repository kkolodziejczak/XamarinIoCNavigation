using System.Linq;
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

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.PopPageAndGoToAsync(page.ToString(), navigationParameters);
        }

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, byte numberOfPagesToPop, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.PopPageAndGoToAsync(numberOfPagesToPop, page.ToString(), navigationParameters);
        }

        public static void InsertPageAfter(this INavigation navigation, Page page, Page after)
        {
            var pages = navigation.NavigationStack.ToList();
            var afterPageIndex = pages.IndexOf(after);

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
