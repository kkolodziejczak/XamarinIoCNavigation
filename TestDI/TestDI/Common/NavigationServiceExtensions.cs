using System.Threading.Tasks;
using TestDI.Navigation;

namespace TestDI.Common
{
    public static class NavigationServiceExtensions
    {
        public static Task GoToAsync(this INavigationService navigationService, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
        {
            return navigationService.GoToAsync(page.ToString(), animated, navigationParameters);
        }

        public static Task GoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.GoToAsync(page.ToString(), navigationParameters);
        }

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
        {
            return navigationService.PopPageAndGoToAsync(page.ToString(), animated, navigationParameters);

        }

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.PopPageAndGoToAsync(page.ToString(), navigationParameters);
        }

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, byte numberOfPagesToPop, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.PopPageAndGoToAsync(numberOfPagesToPop, page.ToString(), navigationParameters);
        }

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, byte numberOfPagesToPop, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
        {
            return navigationService.PopPageAndGoToAsync(numberOfPagesToPop, page.ToString(), animated, navigationParameters);
        }

    }
}
