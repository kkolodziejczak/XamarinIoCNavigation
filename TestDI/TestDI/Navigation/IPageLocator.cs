using Xamarin.Forms;

namespace TestDI.Navigation
{
    public interface IPageLocator
    {
        Page GetPage(string pageName);
    }
}
