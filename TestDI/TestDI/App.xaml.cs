using Autofac;
using TestDI.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
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

            InitializeIoC();

            // Start Application
            ServiceLocalisator.Get<INavigationService>().GoToAsync(ApplicationPage.LoginPage);
        }

        private void InitializeIoC()
        {
            //TODO: Get better grasp on how to get viewModels from other libs and assembles
            var assembly = GetType().Assembly;

            ServiceLocalisator = new ServiceLocalisator(builder =>
            {
                // Register Forms NavigationService
                builder.Register(c => MainPage.Navigation).As<INavigation>();

                // Register all items
                builder.RegisterType<DownloadManager>();
                // ...

                // Register services
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces();

                // Register ViewModels
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("ViewModel"))
                    .OnActivated(async viewModel =>
                    {
                        if (viewModel.Instance is IAsyncInitialization asyncViewModel)
                        {
                            await asyncViewModel.Initialization;
                        }
                    });

                // Register Pages
                builder.RegisterAssemblyTypes(assembly)
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
