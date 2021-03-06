﻿using System;
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
    public class PopAllPagesAndGoToAsyncTests
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
        public Task PopAllPagesAndGoToAsyncSuccessful()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);
                navigation.NavigationStack.Should().HaveCount(2);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, ("key", 4));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
                service.NavigationParameters<int>("key").Should().Be(4);
            });
        }

        [Test]
        public Task PopAllPagesAndGoToAsyncSuccessfulAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();
                await service.GoToAsync(ApplicationPage.LoginPage);
                navigation.NavigationStack.Should().HaveCount(2);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, true, ("key", 4));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
                service.NavigationParameters<int>("key").Should().Be(4);
            });
        }

        [Test]
        public Task PopAllPagesAndGoToAsyncSuccessfulOnlyWithRoot()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, ("key", 4));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
                service.NavigationParameters<int>("key").Should().Be(4);
            });
        }

        [Test]
        public Task PopAllPagesAndGoToAsyncSuccessfulOnlyWithRootAnimated()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                await service.PopAllPagesAndGoToAsync(ApplicationPage.ListViewPage, true, ("key", 4));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(ListViewPage));
                service.NavigationParameters<int>("key").Should().Be(4);
            });
        }

        [Test]
        public Task PopAllPagesAndGoToAsync_Should_InitializeParametersBeforeActions()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                await service.GoToAsync(ApplicationPage.PageWithNavParameterPage, ("key", 4));
                await service.PopAllPagesAndGoToAsync(ApplicationPage.PageWithNavParameterPage, ("key", 5));

                navigation.NavigationStack.Should().HaveCount(1);
                navigation.NavigationStack.Last().Should().BeOfType(typeof(PageWithNavParameterPage));
                var page = navigation.NavigationStack.Last() as PageWithNavParameterPage;
                page.Key.Should().Be(5);
            });
        }

        [Test]
        public Task PopAllPagesAndGoToAsync_List_Should_PushAllPagesAsync()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                await service.GoToAsync(ApplicationPage.PageWithNavParameterPage, ("key", 4));
                await service.PopAllPagesAndGoToAsync(new List<ApplicationPage>
                {
                    ApplicationPage.PageWithNavParameterPage,
                    ApplicationPage.PageWithNavParameterPage,
                }, ("key", 5));

                navigation.NavigationStack.Should().HaveCount(2);
                foreach (var page in navigation.NavigationStack.Skip(1))
                {
                    page.Should().BeOfType(typeof(PageWithNavParameterPage));
                }
            });
        }

        [Test]
        public Task PopAllPagesAndGoToAsync_List_Should_InitializeParametersBeforeActions()
        {
            return ServiceLocator.BeginLifetimeScopeAsync(async serviceLocator =>
            {
                var service = serviceLocator.Get<INavigationService>();
                var navigation = serviceLocator.Get<INavigation>();

                navigation.NavigationStack.Should().HaveCount(1);

                await service.GoToAsync(ApplicationPage.PageWithNavParameterPage, ("key", 4));
                await service.PopAllPagesAndGoToAsync(new List<ApplicationPage>
                {
                    ApplicationPage.PageWithNavParameterPage,
                    ApplicationPage.PageWithNavParameterPage,
                }, ("key", 5));

                var page1 = navigation.NavigationStack.First() as PageWithNavParameterPage;
                var page2 = navigation.NavigationStack.Last() as PageWithNavParameterPage;
                page1.Key.Should().Be(5);
                page2.Key.Should().Be(5);
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
