using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
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
    public class NavigationServiceStrategyBeforePushTests
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
        public async Task NavigationService_StrategyExecuted_beforePush_GoToAsync()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.GoToAsync(ApplicationPage.LoginPage);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_GoToAsyncAnimated()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.GoToAsync(ApplicationPage.LoginPage, true);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_PopPageAndGoToAsync()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_PopPageAndGoToAsyncAnimated()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_PopPageAndGoToAsync_WithAmount()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_PopPageAndGoToAsync_WithAmountAnimated()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.PopPageAndGoToAsync(1, ApplicationPage.LoginPage, true);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_PopAllPagesAndGoToAsync()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            
            // Reset status from before acting
            pushMock.Reset();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);

            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task NavigationService_StrategyExecuted_beforePush_PopAllPagesAndGoToAsyncAnimated()
        {
            var pushMock = new Mock<IPushStrategy>();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>(), pushMock.Object);

            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            // Reset status from before acting
            pushMock.Reset();
            pushMock.Setup(p => p.BeforePushAsync(It.IsAny<Page>()))
                .Returns(Task.CompletedTask);


            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage, true);

            pushMock.Verify(p => p.BeforePushAsync(It.IsAny<Page>()), Times.AtLeastOnce);
        }

    }
}
