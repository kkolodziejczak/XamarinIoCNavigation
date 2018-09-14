using TestDI.Interfaces;
using TestDI.ViewModels;
using Xamarin.Forms;

namespace TestDI.Pages
{
    public class StartPage : Page
    {
        private readonly StartPageViewModel _vm;
        private readonly INavigationService _navigation;

        public StartPage(StartPageViewModel vm, INavigationService navigation)
        {
            BackgroundColor = Color.Red;
            _vm = vm;
            _navigation = navigation;
        }
    }
}
