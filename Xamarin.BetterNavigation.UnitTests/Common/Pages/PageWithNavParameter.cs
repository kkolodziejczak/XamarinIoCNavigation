using Xamarin.BetterNavigation.Core;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Common.Pages
{
    public class PageWithNavParameterPage : Page
    {

        public int Key { get; }

        public PageWithNavParameterPage(INavigationService navigationService)
        {
            Key = navigationService.NavigationParameters<int>("key");
        }
    }
}
