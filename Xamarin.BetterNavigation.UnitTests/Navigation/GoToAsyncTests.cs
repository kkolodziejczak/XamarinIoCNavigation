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
    public class GoToAsyncTests
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
        public Task GoToAsyncSuccessful()
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
        public Task GoToAsyncSuccessfulAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage, true);

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
            });
        }

        [Test]
        public Task GoToAsyncSuccessfulWithParameters()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage, ("SomeKey", 4));

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("SomeKey").Should().Be(4);
            });
        }

        [Test]
        public Task GoToAsyncSuccessfulAnimatedWithParameters()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                await service.GoToAsync(ApplicationPage.LoginPage, true, ("SomeKey", 4));

                navigation.NavigationStack.Should().HaveCount(2);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(LoginPage));
                service.NavigationParameters<int>("SomeKey").Should().Be(4);
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
