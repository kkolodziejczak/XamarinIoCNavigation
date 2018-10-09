using Xamarin.BetterNavigation.Core;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Common.Pages
{
    public class StartPage : Page
    {
        private readonly INavigationService _navigation;

        public StartPage(INavigationService navigation)
        {
            BackgroundColor = Color.Red;
            _navigation = navigation;
        }
    }
}
