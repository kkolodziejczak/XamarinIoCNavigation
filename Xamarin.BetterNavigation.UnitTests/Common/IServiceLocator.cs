using System;
using System.Threading.Tasks;

namespace Xamarin.BetterNavigation.UnitTests.Common
{
    public interface IServiceLocator
    {
        T Get<T>();
        object Get(Type type);
        void BeginLifetimeScope(Action<IServiceLocator> actionToExecute);
        Task BeginLifetimeScope(Func<IServiceLocator, Task> actionToExecute);
    }
}
