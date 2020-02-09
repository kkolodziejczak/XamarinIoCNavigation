using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.BetterNavigation.Core;
using Xamarin.BetterNavigation.UnitTests.Navigation;

namespace Xamarin.BetterNavigation.UnitTests
{
    public static class NavigationServiceExtensions
    {
        public static Task GoToAsync(this INavigationService navigationService, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
            => navigationService.GoToAsync(page.ToString(), animated, navigationParameters);

        public static Task GoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
            => navigationService.GoToAsync(page.ToString(), navigationParameters);

        public static Task GoToAsync(this INavigationService navigationService, IEnumerable<ApplicationPage> pages, bool animated, params (string key, object value)[] navigationParameters)
            => navigationService.GoToAsync(pages.Select(p => p.ToString()), animated, navigationParameters);

        public static Task GoToAsync(this INavigationService navigationService, IEnumerable<ApplicationPage> pages, params (string key, object value)[] navigationParameters)
            => navigationService.GoToAsync(pages.Select(p => p.ToString()), navigationParameters);

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
            => navigationService.PopPageAndGoToAsync(page.ToString(), animated, navigationParameters);

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
            => navigationService.PopPageAndGoToAsync(page.ToString(), navigationParameters);

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, byte numberOfPagesToPop, ApplicationPage page, params (string key, object value)[] navigationParameters)
            => navigationService.PopPageAndGoToAsync(numberOfPagesToPop, page.ToString(), navigationParameters);

        public static Task PopPageAndGoToAsync(this INavigationService navigationService, byte numberOfPagesToPop, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
            => navigationService.PopPageAndGoToAsync(numberOfPagesToPop, page.ToString(), animated, navigationParameters);

        public static Task PopAllPagesAndGoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
            => navigationService.PopAllPagesAndGoToAsync(page.ToString(), navigationParameters);

        public static Task PopAllPagesAndGoToAsync(this INavigationService navigationService, ApplicationPage page, bool animated, params (string key, object value)[] navigationParameters)
            => navigationService.PopAllPagesAndGoToAsync(page.ToString(), animated, navigationParameters);

        public static Task PopAllPagesAndGoToAsync(this INavigationService navigationService, IEnumerable<ApplicationPage> pages, params (string key, object value)[] navigationParameters)
            => navigationService.PopAllPagesAndGoToAsync(pages.Select(p => p.ToString()), navigationParameters);

        public static Task PopAllPagesAndGoToAsync(this INavigationService navigationService, IEnumerable<ApplicationPage> pages, bool animated, params (string key, object value)[] navigationParameters)
            => navigationService.PopAllPagesAndGoToAsync(pages.Select(p => p.ToString()), animated, navigationParameters);
    }
}
