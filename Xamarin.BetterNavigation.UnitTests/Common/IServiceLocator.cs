using System;
using System.Threading.Tasks;

namespace Xamarin.BetterNavigation.UnitTests.Common
{
    public interface IServiceLocator
    {
        T Get<T>();
        object Get(Type type);
        void BeginLifetimeScope(Action<IServiceLocator> scopedServiceLocator);
        Task BeginLifetimeScopeAsync(Func<IServiceLocator, Task> scopedServiceLocator);
    }
}
