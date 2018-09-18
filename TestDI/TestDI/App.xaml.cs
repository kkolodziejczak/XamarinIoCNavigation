using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using TestDI.Common;
using TestDI.Navigation;
using TestDI.Pages;
using Xamarin.Forms;

namespace TestDI
{

    public interface IAsyncInitialization
    {
        Task Initialization { get; }
    }

    public partial class App : Application
    {

        public static IServiceLocator ServiceLocator { get; private set; }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            var AssembliesToImport = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assemblies => assemblies.GetName().Name == "TestDI"); // Add more names there.

            InitializeIoC(AssembliesToImport);

            // Start Application
            var n = MainPage.Navigation;

            var navigationService = ServiceLocator.Get<INavigationService>();
            navigationService.GoToAsync(ApplicationPage.MainMenuPage);
            MainPage.Navigation.PushModalAsync(new LoginPage(navigationService));
            navigationService.GoToAsync(ApplicationPage.MainMenuPage);
            navigationService.PopPageAsync();
            n = MainPage.Navigation;
        }

        private void InitializeIoC(params Assembly[] assemblies)
        {
            ServiceLocator = new ServiceLocator(builder =>
            {
                // Register Forms NavigationService
                builder.RegisterInstance(MainPage.Navigation)
                    .As<INavigation>()
                    .SingleInstance();

                // Register self
                builder.Register(e => ServiceLocator)
                    .As<IServiceLocator>()
                    .SingleInstance();

                // Register all other things
                builder.RegisterType<PageLocator>()
                    .As<IPageLocator>()
                    .SingleInstance();

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
