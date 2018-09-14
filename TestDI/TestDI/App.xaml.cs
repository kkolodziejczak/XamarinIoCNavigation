using System.Reflection;
using Autofac;
using TestDI.Interfaces;
using Xamarin.Forms;

namespace TestDI
{

    public class DownloadManager
    {
        public int _a = 4;
    }

    public partial class App : Application
    {

        public static IServiceLocalisator ServiceLocalisator { get; private set; }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            //TODO: Get better grasp on how to get viewModels from other libs and assembles
            var assembly = GetType().Assembly;
            InitializeIoC(assembly);

            // Start Application
            var navigationService = ServiceLocalisator.Get<INavigationService>();
            navigationService.PopPageAndGoToAsync(ApplicationPage.LoginPage.ToString());
        }

        private void InitializeIoC(params Assembly[] assemblies)
        {
            ServiceLocalisator = new ServiceLocalisator(builder =>
            {
                // Register Forms NavigationService
                builder.RegisterInstance(MainPage.Navigation)
                    .As<INavigation>()
                    .SingleInstance();

                // Register self
                builder.Register(e => ServiceLocalisator)
                    .As<IServiceLocalisator>()
                    .SingleInstance();

                // Register all items
                builder.RegisterType<DownloadManager>();
                // ...

                // Register services
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .SingleInstance();

                // Register ViewModels
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.Name.EndsWith("ViewModel"))
                    .OnActivating(async viewModel =>
                    {
                        if (viewModel.Instance is IAsyncInitialization asyncViewModel)
                        {
                            await asyncViewModel.Initialization;
                        }
                    });

                // Register Pages
                builder.RegisterAssemblyTypes(assemblies)
                   .Where(t => t.Name.EndsWith("Page"));
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
