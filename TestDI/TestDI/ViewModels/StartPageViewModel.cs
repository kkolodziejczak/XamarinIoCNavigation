using System.Threading.Tasks;
using TestDI.Interfaces;

namespace TestDI.ViewModels
{
    public class StartPageViewModel : BaseViewModel, IAsyncInitialization
    {
        private readonly IAlertService _alertService;
        public DownloadManager Manager { get; }

        public Task Initialization { get; }

        public StartPageViewModel(IAlertService alertService, DownloadManager manager)
        {
            _alertService = alertService;
            Manager = manager;
            Initialization = Init();
        }

        private Task Init()
        {
            return Task.CompletedTask;
        }
    }
}
