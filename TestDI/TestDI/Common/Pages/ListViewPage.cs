using System.Threading.Tasks;
using TestDI.Common;
using Xamarin.Forms;

namespace TestDI.Pages
{
    public class ListViewPage : Page, IAsyncInitialization
    {
        private readonly ListPageViewModel _viewModel;
        public Task Initialization { get; }

        public ListViewPage(ListPageViewModel viewModel)
        {
            _viewModel = viewModel;
            Initialization = Init();
        }

        private Task Init()
        {
            return Task.CompletedTask;
        }

    }
}
