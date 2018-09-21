using Xamarin.BetterNavigation.Core;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Common.Pages
{
    public class LoginPage : Page
    {
        private readonly INavigationService _navigation;

        public LoginPage(INavigationService navigation)
        {
            BackgroundColor = Color.Blue;
            _navigation = navigation;
        }
    }
}
