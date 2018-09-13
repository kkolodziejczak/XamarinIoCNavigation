using TestDI.Interfaces;
using TestDI.ViewModels;
using Xamarin.Forms;

namespace TestDI.Pages
{
    public class LoginPage : Page
    {
        private readonly LoginPageViewModel _vm;
        private readonly INavigationService _navigation;

        public LoginPage(LoginPageViewModel vm, INavigationService navigation)
        {
            _vm = vm;
            _navigation = navigation;
        }
    }
}
