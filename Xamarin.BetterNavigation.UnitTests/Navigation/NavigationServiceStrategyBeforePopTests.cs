using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
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
    public class NavigationServiceStrategyBeforePopTests
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


        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageToRootAsync_AlreadyOnTheRoot()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.PopPageToRootAsync();

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.Never);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageToRootAsync_WithPageToPop()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync();

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageToRootAsync_WithTwoPagesToPop()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync();

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageToRootAsyncAnimated_AlreadyOnTheRoot()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.PopPageToRootAsync(true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.Never);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageToRootAsyncAnimated_WithPageToPop()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync(true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageToRootAsyncAnimated_WithTwoPagesToPop()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageToRootAsync(true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAsync()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync();

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAsyncAnimated()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync(true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAsync_WithAmount()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync(1);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAsync_WithAmountAnimated()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);
            await service.GoToAsync(ApplicationPage.LoginPage);

            await service.PopPageAsync(1, true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAndGoToAsync()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAndGoToAsyncAnimated()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAndGoToAsync_WithAmount()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAndGoToAsync_WithAmountAnimated()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage, true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopPageAsync_WithAmount_ToPopManyPages([Values(1, 4, 10, 15)]int amountOfPagesToPop)
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            for (int i = 0; i < amountOfPagesToPop; i++)
            {
                await service.GoToAsync(ApplicationPage.ListViewPage);
            }

            await service.PopPageAndGoToAsync((byte)amountOfPagesToPop, ApplicationPage.LoginPage, true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.Exactly(amountOfPagesToPop));
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopAllPagesAndGoToAsync()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.Exactly(4 + 1)); // +1 For the 1st page after Navigation is initialized
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePop_PopAllPagesAndGoToAsyncAnimated()
        {
            var popMock = new Mock<IPopStrategy>();
            popMock.Setup(p => p.BeforePopAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), popMock.Object);

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage, true);

            popMock.Verify(p => p.BeforePopAsync(It.IsAny<Page>()), Times.Exactly(4 + 1)); // +1 For the 1st page after Navigation is initialized
        }
    }
}
