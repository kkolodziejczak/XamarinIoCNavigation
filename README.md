# XamarinIoCNavigation
Concept on how to create your own navigation using IoC container in Xamarin. Based on that concept NuGet Package was created, and can be used in any Xamarin Forms application.

## Shortcuts
- [Basic usage](#Basic-usage)
- [All possibilities](#all-possibilities-of-xamarinbetternavigation)

[![License](http://img.shields.io/:license-mit-blue.svg)](https://github.com/kkolodziejczak/XamarinIoCNavigation/blob/master/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/Xamarin.BetterNavigation.Forms)](https://www.nuget.org/packages/Xamarin.BetterNavigation.Forms/)


 ## Build Status ##

| Master    | Develop             |
|--------------|--------------|
|  [![Build status](https://ci.appveyor.com/api/projects/status/is5sv1vqq7x9v7ty/branch/master?svg=true)](https://ci.appveyor.com/project/kkolodziejczak/xamariniocnavigation/branch/master)<br>[![Coverage Status](https://coveralls.io/repos/github/kkolodziejczak/XamarinIoCNavigation/badge.svg?branch=master)](https://coveralls.io/github/kkolodziejczak/XamarinIoCNavigation?branch=master) | [![Build status](https://ci.appveyor.com/api/projects/status/is5sv1vqq7x9v7ty/branch/develop?svg=true)](https://ci.appveyor.com/project/kkolodziejczak/xamariniocnavigation/branch/develop) <br> [![Coverage Status](https://coveralls.io/repos/github/kkolodziejczak/XamarinIoCNavigation/badge.svg?branch=develop)](https://coveralls.io/github/kkolodziejczak/XamarinIoCNavigation?branch=develop) |

## Packages ##

 Package name | Current version |
-------------------------------------------|-----------------------------|
 `Xamarin.BetterNavigation.Forms` | [![NuGet](https://img.shields.io/nuget/v/Xamarin.BetterNavigation.Forms.svg)](https://www.nuget.org/packages/Xamarin.BetterNavigation.Forms/)|
 `Xamarin.BetterNavigation.Core`    | [![NuGet](https://img.shields.io/nuget/v/Xamarin.BetterNavigation.Core.svg)](https://www.nuget.org/packages/Xamarin.BetterNavigation.Core/) | 

## Packages Content ##

`Xamarin.BetterNavigation.Forms` contains implementation of navigation service that is used to navigate inside your application. 

`Xamarin.BetterNavigation.Core` have `INavigationService` interface to use inside your library that doesn't have reference to Xamarin. This helps to keep all references to minimum and invert dependencies of navigation in code.

## Basic usage ##
To use `NavigationService` you need to have Xamarin `NavigationPage` from it you can get `INavigation` property that is used to navigate through application. After that you need to create class that implements `Xamarin.BetterNavigation.Forms.IPageLocator` interface. This class converts strings to Xamarin `Page`'s. 
### Page Locator ###
#### TestDI.Common.PageLocator.cs ####
```C#
public class PageLocator : IPageLocator
{
    private readonly IServiceLocator _serviceLocator;

    public static readonly Dictionary<string, Type> PageMap 
        = new Dictionary<string, Type>
    {
        { ApplicationPage.LoginPage.ToString(), typeof(LoginPage) },
        { ApplicationPage.ListViewPage.ToString(), typeof(ListViewPage) },
    };

    public PageLocator(IServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator;
    }

    public Page GetPage(string pageName)
    {
        return (Page)_serviceLocator.Get(PageMap[pageName]);
    }

    public string GetPageName(Page page)
    {
        foreach (var registeredPage in PageMap)
        {
            if (page.GetType().IsInstanceOfType(registeredPage.Value))
            {
                return registeredPage.Key;
            }
        }

        return default;
    }    
}
```
### Creation of Navigation Service ###
```C#
public static IServiceLocator ServiceLocator { get; private set; }

public App()
{
    InitializeComponent();

    MainPage = new NavigationPage(new MainPage());

    var service = 
        new NavigationService(MainPage.Navigation, new PageLocator(ServiceLocator));

    service.GoToAsync("SomePage");
}
```

### Strategies for Push and Pop ###
Since version 1.2.0 You can pass `IPopStrategy`, and `IPushStrategy` to the Constructor. Strategies will be used before page is pushed to/popped from the navigation stack.
By default NavigationService uses `DoNothingStrategy`.

```C#
namespace Xamarin.BetterNavigation.Forms
{
    public interface IPopStrategy
    {
        Task BeforePopAsync(Page pageToPop);
    }

    public interface IPushStrategy
    {
        Task BeforePushAsync(Page pageToPush);
    }
}
```
### Pop strategy example ###
```C#

public class DisposePageStrategy : IPopStrategy
{
    public async Task BeforePopAsync(Page pageToPop)
    {
        if (pageToPop is IDisposable disposablePage)
        {
            disposablePage.Dispose();
        }
    }
}

public App()
{
    InitializeComponent();

    MainPage = new NavigationPage(new MainPage());

    var service = 
        new NavigationService(MainPage.Navigation, new PageLocator(ServiceLocator), new DisposePageStrategy());

    service.GoToAsync("SomePage");
}
```
### Some Improvements ###
To keep this package universal as possible. We use strings to navigate but they are not as easy to use as enums. There is easy fix for that. You can create _extension methods_ for all methods of `INavigationService` and use enum instead of string for all pages that You want to use. All extension methods are created and ready to being copied in [this file](TestDI/TestDI/Common/NavigationServiceExtensions.cs).
#### TestDI.Common.NavigationServiceExtensions.cs ####
```C#
    public enum ApplicationPage
    {
        LoginPage,
        ListViewPage,
    }

    public static class NavigationServiceExtensions
    {
        public static Task GoToAsync(this INavigationService navigationService, 
            ApplicationPage page,
            bool animated, 
            params (string key, object value)[] navigationParameters)
        {
            return navigationService.GoToAsync(page.ToString(),
                animated,
                navigationParameters);
        }

        ///
        /// ... implementations ...
        ///

    }
```


## All possibilities of Xamarin.BetterNavigation ##
```C#
namespace Xamarin.BetterNavigation.Core
{
    public interface INavigationService
    {
        CancellationToken CancellationToken { get; }

        string PeekPage();

        T NavigationParameters<T>(string parameterKey);

        bool ContainsParameterKey(string parameterKey);

        bool TryGetValue<T>(string parameterKey, out T value);

        Task PopPageToRootAsync();

        Task PopPageToRootAsync(bool animated);

        Task PopPageAsync();

        Task PopPageAsync(bool animated);

        Task PopPageAsync(byte amount);

        Task PopPageAsync(byte amount, bool animated);

        Task PopAllPagesAndGoToAsync(string pageName,
            params (string key, object value)[] navigationParameters);

        Task PopAllPagesAndGoToAsync(string pageName,
            bool animated,
            params (string key, object value)[] navigationParameters);

        Task PopAllPagesAndGoToAsync(IEnumerable<string> pageNames,
            params (string key, object value)[] navigationParameters);

        Task PopAllPagesAndGoToAsync(IEnumerable<string> pageNames,
            bool animated,
            params (string key, object value)[] navigationParameters);

        Task GoToAsync(string pageName, 
            params (string key, object value)[] navigationParameters);

        Task GoToAsync(string pageName,
            bool animated,
            params (string key, object value)[] navigationParameters);

        Task GoToAsync(IEnumerable<string> pageNames,
            params (string key, object value)[] navigationParameters);

        Task GoToAsync(IEnumerable<string> pageNames,
            bool animated,
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(string pageName, 
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(string pageName,
            bool animated, 
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(IEnumerable<string> pageNames,
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(IEnumerable<string> pageNames,
            bool animated,
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(byte amount,
            string pageName, 
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(byte amount,
            string pageName,
            bool animated,  
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(byte amount,
            IEnumerable<string> pageNames,
            params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(byte amount,
            IEnumerable<string> pageNames,
            bool animated,
            params (string key, object value)[] navigationParameters);
    }
}
```
