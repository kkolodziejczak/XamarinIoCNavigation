using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Common.Pages
{
    public class ListViewPage : Page
    {
        public Task Initialization { get; }

        public ListViewPage()
        {
            Initialization = Init();
        }

        private Task Init()
        {
            return Task.CompletedTask;
        }

    }
}
