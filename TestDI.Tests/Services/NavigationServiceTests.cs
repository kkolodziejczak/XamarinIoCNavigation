using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using TestDI.Interfaces;
using TestDI.Pages;
using TestDI.Services;
using TestDI.Tests.Fakes;
using TestDI.Tests.Fakes.FakeXamarinForms;
using Xamarin.Forms;

namespace TestDI.Tests.Services
{
    [TestFixture]
    public class NavigationServiceTests
    {
        public static IServiceLocalisator ServiceLocalisator { get; private set; }
        public INavigation Navigation { get; protected set; }

        // create dummy item to ensure that Assembly is added
        private NavigationService _dummyInstance = new NavigationService(null, null);

        [OneTimeSetUp]
        public void ResourcesFixture()
        {
            // Always call this to ensure that XamarinForms is initialized!
            FakeXamarinForms.Init();
        }

        [SetUp]
        public void Setup()
        {
            var testedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == "TestDI");

            Navigation = new FakeNavigation(new MainPage());
            InitializeIoC(testedAssembly);
        }

        [Test]
        public async Task NavigationService_PopPagesToRoot()
        {
            var service = ServiceLocalisator.Get<INavigationService>();
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync();

            Navigation.NavigationStack.Should().HaveCount(1);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
        }

        [Test]
        public async Task NavigationService_PopPage()
        {
            var service = ServiceLocalisator.Get<INavigationService>();
            await service.GoToAsync(ApplicationPage.LoginPage);

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
        public async Task NavigationService_PopPageAndGoTo()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.PopPageAndGoToAsync(ApplicationPage.ListViewPage);

            Navigation.NavigationStack.Should().HaveCount(2);
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
        }

        [Test]
        public async Task NavigationService_PopPageAndGoToPage_BackTwoPages()
        {
            var service = ServiceLocalisator.Get<INavigationService>();
            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.MainMenuPage);

            await service.PopPageAndGoToAsync(2, ApplicationPage.ListViewPage);

            Navigation.NavigationStack.Should().HaveCount(2);
            Navigation.NavigationStack.First().Should().BeOfType(typeof(MainPage));
            Navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
        }

        [Test]
        public async Task NavigationService_GoToPage_ReadNavigationProperties()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            service.NavigationParameters<int>("documentCount").Should().Be(5);
        }

        [Test]
        public async Task NavigationService_GoToPage_ReadNavigationProperties_keyNoFound()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.Throws<KeyNotFoundException>(() =>
            {
                service.NavigationParameters<int>("documentCountSomethingElse");
            });
        }

        [Test]
        public async Task NavigationService_GoToPage_ReadNavigationProperties_InvalidCast()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.Throws<InvalidCastException>(() =>
            {
                service.NavigationParameters<string>("documentCount");
            });
        }

        [Test]
        public async Task NavigationService_PopTooManyPages()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.Throws<ArgumentOutOfRangeException>(() => service.PopPageAsync(4));
        }

        [Test]
        public void NavigationService_PopTooManyPagesAndGoToPage()
        {
            var service = ServiceLocalisator.Get<INavigationService>();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, ("documentCount", 5)));
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
