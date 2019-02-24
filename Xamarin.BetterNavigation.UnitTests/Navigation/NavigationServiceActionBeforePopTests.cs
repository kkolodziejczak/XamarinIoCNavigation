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
    public class NavigationServiceActionBeforePopTests
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
        public async Task NavigationService_ActionExecuted_beforePop_PopPageToRootAsync_AlreadyOnTheRoot()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.PopPageToRootAsync();

            popInvoked.Should().Be(false);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageToRootAsync_WithPageToPop()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync();

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageToRootAsync_WithTwoPagesToPop()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync();

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageToRootAsyncAnimated_AlreadyOnTheRoot()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.PopPageToRootAsync(true);

            popInvoked.Should().Be(false);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageToRootAsyncAnimated_WithPageToPop()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync(true);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageToRootAsyncAnimated_WithTwoPagesToPop()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync(true);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAsync()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync();

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAsyncAnimated()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync(true);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAsync_WithAmount()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync(1);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAsync_WithAmountAnimated()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync(1, true);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAndGoToAsync()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAndGoToAsyncAnimated()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAndGoToAsync_WithAmount()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAndGoToAsync_WithAmountAnimated()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked = true; }, null);

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage, true);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopPageAsync_WithAmount_ToPopManyPages([Values(1, 4, 10, 15)]int amountOfPagesToPop)
        {
            var popInvoked = 0;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                p => { popInvoked += 1; }, null);

            for (int i = 0; i < amountOfPagesToPop; i++)
            {
                await service.GoToAsync(ApplicationPage.ListViewPage);
            }

            await service.PopPageAndGoToAsync((byte)amountOfPagesToPop, ApplicationPage.LoginPage, true);

            popInvoked.Should().Be(amountOfPagesToPop);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopAllPagesAndGoToAsync()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { popInvoked = true; });

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting
            popInvoked = false;


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage);

            popInvoked.Should().Be(true);
        }

        [Test]
        public async Task NavigationService_ActionExecuted_beforePop_PopAllPagesAndGoToAsyncAnimated()
        {
            var popInvoked = false;
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(),
                null, p => { popInvoked = true; });

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting
            popInvoked = false;


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage, true);

            popInvoked.Should().Be(true);
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
