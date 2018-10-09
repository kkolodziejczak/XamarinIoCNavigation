using System;

namespace Xamarin.BetterNavigation.UnitTests.Common
{
    public interface IServiceLocator
    {
        T Get<T>();
        object Get(Type type);
    }
}
