using System.Threading.Tasks;
using TestDI.Interfaces;

namespace TestDI
{
    public static class NavigationPageOverride
    {
        public static Task GoToAsync(this INavigationService navigationService, ApplicationPage page, params (string key, object value)[] navigationParameters)
        {
            return navigationService.GoToAsync(page.ToString(), navigationParameters);
        }
    }
}
