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
    public class PopPageAsyncTests
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
        public Task PopPageAsyncSuccessful()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopPageAsync();

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopPageAsync(true);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulOnlyRootPage()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                try
                {
                    await service.PopPageAsync();
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<ArgumentOutOfRangeException>()
                        .Which.Message.Should().Contain("You want to remove too many pages from the Navigation Stack.");
                }
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulOnlyRootPageAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                try
                {
                    await service.PopPageAsync(true);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<ArgumentOutOfRangeException>()
                        .Which.Message.Should().Contain("You want to remove too many pages from the Navigation Stack.");
                }
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulMoreThanOnePageOnTheStack()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                navigation.NavigationStack.Should().HaveCount(2);

                await service.PopPageAsync();

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulMoreThanOnePageOnTheStackAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                navigation.NavigationStack.Should().HaveCount(2);
                await service.PopPageAsync(true);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulPop2Pages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(3);
                await service.PopPageAsync(2);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task PopPageAsyncSuccessfulPop2PagesWhenThereAreOnly2PagesOnTheStack()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                navigation.NavigationStack.Should().HaveCount(2);
                try
                {
                    await service.PopPageAsync(2);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<ArgumentOutOfRangeException>();
                }
            });
        }

        [Test]
        public Task PopPageAsyncPopping0PagesThrowsException()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();

                try
                {
                    await service.PopPageAsync(0);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<ArgumentOutOfRangeException>()
                        .Which.Message.Should().Contain("You want to remove 0 pages from the Navigation Stack.");
                }
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
