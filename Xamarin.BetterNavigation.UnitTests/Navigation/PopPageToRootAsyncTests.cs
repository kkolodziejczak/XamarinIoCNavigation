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
    public class PopPageToRootAsyncTests
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
        public Task PopPageToRootAsyncSuccessful([Values(0, 1, 5)]int pageAmount)
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
        public Task PopPageToRootAsyncSuccessfulAnimated([Values(0, 1, 5)]int pageAmount)
        {
            var s = ServiceLocator.Get<IServiceLocator>();
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
