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
    public class NavigationServiceCancellationTokenTests
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
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_PopPageToRootAsync()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.LoginPage);

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageToRootAsync();

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_PopPageToRootAsyncAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.LoginPage);

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageToRootAsync(true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopPageToRootAsync_WithOnlyOnePageOnTheStack()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageToRootAsync();

            // Assert
            token.IsCancellationRequested.Should().BeFalse();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopPageToRootAsync_WithOnlyOnePageOnTheStackAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageToRootAsync(true);

            // Assert
            token.IsCancellationRequested.Should().BeFalse();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopPageAsync()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.ListViewPage);
            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAsync();

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopPageAsyncAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.ListViewPage);
            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAsync(true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopPageAsyncWithAmount()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAsync(2);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopPageAsyncWithAmountAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAsync(2, true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopAllPagesAndGoToAsync()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }


        [Test]
        public async Task NavigationService_CancellationToken_ShouldNotBeCanceled_When_PopAllPagesAndGoToAsyncAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.ListViewPage);
            await service.GoToAsync(ApplicationPage.ListViewPage);
            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopAllPagesAndGoToAsync(ApplicationPage.LoginPage, true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_GoToAsync()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.GoToAsync(ApplicationPage.LoginPage);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_GoToAsyncAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.GoToAsync(ApplicationPage.LoginPage, true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_PopPageAndGoToAsync()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.LoginPage);

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_PopPageAndGoToAsyncAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.LoginPage);

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAndGoToAsync(ApplicationPage.LoginPage, true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_PopPageAndGoToAsyncAmount()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.LoginPage);

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

        [Test]
        public async Task NavigationService_CancellationToken_ShouldBeCanceled_When_PopPageAndGoToAsyncAmountAnimated()
        {
            // Arrange
            var service = new NavigationService(ServiceLocator.Get<INavigation>(), ServiceLocator.Get<IPageLocator>());
            await service.GoToAsync(ApplicationPage.LoginPage);
            await service.GoToAsync(ApplicationPage.LoginPage);

            var token = service.CancellationToken;
            token.IsCancellationRequested.Should().BeFalse();

            // Act
            await service.PopPageAndGoToAsync(2, ApplicationPage.LoginPage, true);

            // Assert
            token.IsCancellationRequested.Should().BeTrue();
        }

    }
}
