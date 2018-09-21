using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{
    public interface IPageLocator
    {
        Page GetPage(string pageName);
    }
}
