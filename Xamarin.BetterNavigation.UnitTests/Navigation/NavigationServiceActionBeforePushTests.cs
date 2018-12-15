using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using Xamarin.BetterNavigation.Forms;
using Xamarin.BetterNavigation.UnitTests.Common;
using Xamarin.BetterNavigation.UnitTests.Common.Pages;
using Xamarin.BetterNavigation.UnitTests.Fakes.FakeXamarinForms;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Navigation
{
    [TestFixture]
    [NonParallelizable]
    public class NavigationServiceActionBeforePushTests
    {

        public static IServiceLocator ServiceLocator { get; private set; }
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
                .Where(assembly => assembly.GetName().Name.Contains("Xamarin.BetterNavigation"));

            Navigation = new NavigationPage(new MainPage()).Navigation;
            InitializeIoC(testedAssembly.ToArray());
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_GoToAsync()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.GoToAsync(ApplicationPage.LoginPage);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_GoToAsyncAnimated()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.GoToAsync(ApplicationPage.LoginPage, true);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_PopPageAndGoToAsync()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_PopPageAndGoToAsyncAnimated()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_PopPageAndGoToAsync_WithAmount()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_PopPageAndGoToAsync_WithAmountAnimated()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage, true);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_PopAllPagesAndGoToAsync()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting
            pushInvoked = false;


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage);

            pushInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePush_PopAllPagesAndGoToAsyncAnimated()
        {
            var pushInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { pushInvoked = true; });

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting
            pushInvoked = false;


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage, true);

            pushInvoked.Should().Be(true);
        }

        private void InitializeIoC(params Assembly[] assemblies)
        {
            ServiceLocator = new ServiceLocator(builder =>
            {
                // Register Forms NavigationService
                builder.RegisterInstance(Navigation)
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
                    .Where(t => t.Name.EndsWith("ViewModel"));

                // Register Pages
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.Name.EndsWith("Page"));
            });
        }
    }
}
