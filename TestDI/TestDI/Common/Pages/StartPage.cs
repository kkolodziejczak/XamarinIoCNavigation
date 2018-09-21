using Xamarin.BetterNavigation.Core;
using Xamarin.Forms;

namespace TestDI.Pages
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
