using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using TestDi.UnitTests.Fakes;
using TestDI;
using TestDI.Interfaces;
using TestDI.Pages;
using TestDI.Services;
using Xamarin.Forms;

namespace TestDi.UnitTests.Services
{

    [TestFixture]
    public class NavigationServiceTests
    {
        public static IServiceLocalisator ServiceLocalisator { get; private set; }
        public INavigation Navigation { get; protected set; }
        private NavigationService _dummyInstance = new NavigationService(null, null);

        [OneTimeSetUp]
        public void ResourcesFixture()
        {
            MockForms.Init();
        }

        [SetUp]
        public void Setup()
        {
            var testedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == "TestDI");

            Navigation = new FakeNavigation();
            Navigation.PushAsync(new MainPage());
            InitializeIoC(testedAssembly);
        }

        [Test]
        public async Task NavigationService_PopPagesToRoot()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);
            Navigation.NavigationStack.Should().HaveCountGreaterThan(1);

            await service.PopPageToRootAsync();

            Navigation.NavigationStack.Should().HaveCount(1);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
        }

        [Test]
        public async Task NavigationService_PopPage()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);
            Navigation.NavigationStack.Should().HaveCountGreaterThan(1);

            await service.PopPageAsync();

            Navigation.NavigationStack.Should().HaveCount(1);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
        }

        [Test]
        public async Task NavigationService_PopPage_Twice()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.SideBar);

            await service.PopPageAsync(2);

            Navigation.NavigationStack.Should().HaveCount(1);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
        }

        [Test]
        public async Task NavigationService_GoTo()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);

            Navigation.NavigationStack.Should().HaveCount(2);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
        }

        [Test]
        public async Task NavigationService_PopOnePageAndGoTo_withOnlyOnePageOnTheStack()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

            Navigation.NavigationStack.Should().HaveCount(1);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
        }

        [Test]
        public async Task NavigationService_PopOnePageAndGoTo()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.PopPageAndGoToAsync(ApplicationPage.ListViewPage);

            Navigation.NavigationStack.Should().HaveCount(2);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
        }


        #region Utils

        private void InitializeIoC(params Assembly[] assemblies)
        {
            ServiceLocalisator = new ServiceLocalisator(builder =>
            {
                // Register Forms NavigationService
                builder.RegisterInstance(Navigation)
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

        #endregion
    }


}
