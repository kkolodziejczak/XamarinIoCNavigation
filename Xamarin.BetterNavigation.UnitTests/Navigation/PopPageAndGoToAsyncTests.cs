using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using Xamarin.BetterNavigation.Core;
using Xamarin.BetterNavigation.Forms;
using Xamarin.BetterNavigation.UnitTests.Common;
using Xamarin.BetterNavigation.UnitTests.Common.Pages;
using Xamarin.BetterNavigation.UnitTests.Fakes.FakeXamarinForms;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Navigation
{
    [TestFixture]
    public class PopPageAndGoToAsyncTests
    {
        public IServiceLocator ServiceLocator { get; private set; }

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
                .Where(assembly => assembly.GetName().Name.Contains("Xamarin.BetterNavigation"));

            InitializeIoC(testedAssembly.ToArray());
        }

        [Test]
        public Task PopPageAndGoToAsyncExchangeRootPage()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncExchangeRootPageAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncChangeCurrentPage()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.ListViewPage);

                await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncChangeCurrentPageAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.ListViewPage);

                await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncPopTooManyPages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                await service.GoToAsync(ApplicationPage.ListViewPage);

                try
                {
                    await service.PopPageAndGoToAsync(5, ApplicationPage.LoginPage);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<ArgumentOutOfRangeException>();
                }
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncPopTooManyPagesAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                await service.GoToAsync(ApplicationPage.ListViewPage);

                try
                {
                    await service.PopPageAndGoToAsync(5, ApplicationPage.LoginPage, true);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<ArgumentOutOfRangeException>();
                }
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncPopExactNumberOfPages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(2);
                await service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncPopExactNumberOfPagesAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(2);
                await service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, true, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncPopManyPages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.ListViewPage);
                await service.GoToAsync(ApplicationPage.MainMenuPage);

                await service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.First().Should().BeOfType(typeof(MainPage));
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsyncPopManyPagesAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.ListViewPage);
                await service.GoToAsync(ApplicationPage.MainMenuPage);

                await service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, true, ("key", 3));

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.First().Should().BeOfType(typeof(MainPage));
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("key").Should().Be(3);
            });
        }

        [Test]
        public Task PopPageAndGoToAsync_Should_InitializeNavigationParametersBeforeCreatingPage()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                await service.GoToAsync(ApplicationPage.PageWithNavParameterPage, ("key", 4));
                await service.PopPageAndGoToAsync(ApplicationPage.PageWithNavParameterPage, ("key", 5));

                var page = navigation.NavigationStack.Last() as PageWithNavParameterPage;
                page.Key.Should().Be(5);
            });
        }

        private void InitializeIoC(params Assembly[] assemblies)
        {
            ServiceLocator = new ServiceLocator(builder =>
            {
                // Register Forms NavigationService
                builder.Register(e => new NavigationPage(e.Resolve<MainPage>()).Navigation)
                    .As<INavigation>()
                    .InstancePerLifetimeScope();

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
                    .InstancePerLifetimeScope();

                // Register ViewModels
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.Name.EndsWith("ViewModel"));

                // Register Pages
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.Name.EndsWith("Page"));
            });
        }
    }
}
