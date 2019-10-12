using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{

    /// <summary>
    /// <see cref="IPageLocator"/> converts string PageName to Xamarin Page.
    /// </summary>
    public interface IPageLocator
    {
        /// <summary>
        /// Returns Page based on string <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">Name of the page to return.</param>
        /// <returns></returns>
        Page GetPage(string pageName);

        /// <summary>
        /// Peeks navigation stack
        /// </summary>
        /// <returns></returns>
        string GetPageName(Page page);
    }
}
