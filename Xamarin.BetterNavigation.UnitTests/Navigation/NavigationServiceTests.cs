using System;
using System.Collections.Generic;
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
    public class NavigationServiceTests
    {
        public static IServiceLocator ServiceLocator { get; private set; }

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
        public Task NavigationService_PopPagesToRoot([Values(0, 1, 5, 10)]int pageAmount)
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                for (var i = 0; i < pageAmount; i++)
                    await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopPageToRootAsync();

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task NavigationService_PopPagesToRootAnimated([Values(0, 1, 5, 10)]int pageAmount)
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                for (var i = 0; i < pageAmount; i++)
                    await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopPageToRootAsync(true);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task NavigationService_PopPage()
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
        public Task NavigationService_PopPageAnimated()
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
        public Task NavigationService_PopPage_Twice()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.SideBar);

                await service.PopPageAsync(2);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(MainPage));
            });
        }

        [Test]
        public Task NavigationService_GoTo()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage);

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
            });
        }

        [Test]
        public Task NavigationService_PopOnePageAndGoTo_withOnlyOnePageOnTheStack()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
            });

        }

        [Test]
        public Task NavigationService_PopPageAndGoTo()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.PopPageAndGoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
            });

        }

        [Test]
        public Task NavigationService_PopPageAndGoToAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.PopPageAndGoToAsync(ApplicationPage.ListViewPage, true);

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
            });

        }

        [Test]
        public Task NavigationService_PopPageAndGoToPage_BackTwoPages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = ServiceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.MainMenuPage);

                await service.PopPageAndGoToAsync(2, ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.First().Should().BeOfType(typeof(MainPage));
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public async Task NavigationService_GoToPage_ReadNavigationProperties()
        {
            var service = ServiceLocator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            service.NavigationParameters<int>("documentCount").Should().Be(5);
        }

        [Test]
        public async Task NavigationService_GoToPage_ReadNavigationProperties_keyNoFound()
        {
            var service = ServiceLocator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.Throws<KeyNotFoundException>(() => service.NavigationParameters<int>("documentCountSomethingElse"));
        }

        [Test]
        public async Task NavigationService_GoToPage_ReadNavigationProperties_InvalidCast()
        {
            var service = ServiceLocator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.Throws(typeof(InvalidCastException), () => service.NavigationParameters<string>("documentCount"));
        }

        [Test]
        public async Task NavigationService_GoToPage_CheckIfNavigationPropertyExists_Positive()
        {
            var service = ServiceLocator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            service.ContainsParameterKey("documentCount").Should().Be(true);
        }

        [Test]
        public async Task NavigationService_GoToPage_CheckIfNavigationPropertyExists_Negative()
        {
            var service = ServiceLocator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage);

            service.ContainsParameterKey("documentCount").Should().Be(false);
        }

        [Test]
        public void NavigationService_GoToPage_CheckIfNavigationPropertyExists_ThrowsException()
        {
            var service = ServiceLocator.Get<INavigationService>();

            Assert.Throws<ArgumentNullException>(() => service.ContainsParameterKey(null));
        }

        [Test]
        public async Task NavigationService_GoToPage_NavigationProperty_TryGetValue_Positive()
        {
            var service = ServiceLocator.Get<INavigationService>();
            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            var result = service.TryGetValue("documentCount", out int count);

            result.Should().Be(true);
        }

        [Test]
        public void NavigationService_GoToPage_NavigationProperty_TryGetValue_Negative()
        {
            var service = ServiceLocator.Get<INavigationService>();

            var result = service.TryGetValue("documentCount", out int count);

            result.Should().Be(false);
        }

        [Test]
        public async Task NavigationService_GoToPage_NavigationProperty_TryGetValue_InvalidReturnType()
        {
            var service = ServiceLocator.Get<INavigationService>();
            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.Throws<InvalidCastException>(() => service.TryGetValue("documentCount", out string count));
        }


        [Test]
        public void NavigationService_GoToPage_NavigationProperty_TryGetValue_InvalidKey()
        {
            var service = ServiceLocator.Get<INavigationService>();

            Assert.Throws<ArgumentNullException>(() => service.TryGetValue(null, out string count));
        }

        [Test]
        public async Task NavigationService_PopTooManyPages()
        {
            var service = ServiceLocator.Get<INavigationService>();

            await service.GoToAsync(ApplicationPage.LoginPage, ("documentCount", 5));

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.PopPageAsync(4));
        }

        [Test]
        public void NavigationService_PopTooManyPagesAndGoToPage()
        {
            var service = ServiceLocator.Get<INavigationService>();

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, ("documentCount", 5)));
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoTo()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = ServiceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoTo_OnlyWithRootPage()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoTo_WithTwoPages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoTo_WithFivePages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoToAnimated_OnlyWithRootPage()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, true);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoToAnimated_WithTwoPages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, true);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        [Test]
        public Task NavigationService_PopAllPagesAndGoToAnimated_WithFivePages()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);
                await service.GoToAsync(ApplicationPage.LoginPage);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, true);

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.First().Should().BeOfType(typeof(ListViewPage));
            });
        }

        #region Utils

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

        #endregion
    }
}
