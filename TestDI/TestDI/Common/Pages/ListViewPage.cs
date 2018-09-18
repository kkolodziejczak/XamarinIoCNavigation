using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestDI.Pages
{
    public class ListViewPage : Page, IAsyncInitialization
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
