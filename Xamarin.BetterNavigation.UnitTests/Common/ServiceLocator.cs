using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;

namespace Xamarin.BetterNavigation.UnitTests.Common
{
    public class ServiceLocator : IServiceLocator,  IDisposable
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ContainerBuilder _builder;
        private readonly IContainer _containter;

        public ServiceLocator(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public ServiceLocator(Action<ContainerBuilder> builder)
        {
            _builder = new ContainerBuilder();
            builder(_builder);
            _containter = _builder.Build();
        }

        /// <exception cref="ComponentNotRegisteredException"/>
        /// <exception cref="DependencyResolutionException"/>
        public T Get<T>()
        {
            try
            {
                if (_lifetimeScope != null)
                {
                    return _lifetimeScope.Resolve<T>();
                }
                return _containter.Resolve<T>();
            }
            catch (ComponentNotRegisteredException e)
            {
                Debug.WriteLine("~~!~~~~!~~~~!~~~~!~~~~!~~~~!~~");
                Debug.WriteLine("Component not registered! \n" + e.Message);
                Debug.WriteLine("~~!~~~~!~~~~!~~~~!~~~~!~~~~!~~");
                throw;
            }
            catch (DependencyResolutionException e)
            {
                throw;
            }
        }

        /// <exception cref="ComponentNotRegisteredException"/>
        /// <exception cref="DependencyResolutionException"/>
        public object Get(Type type)
        {
            try
            {
                if (_lifetimeScope != null)
                {
                    return _lifetimeScope.Resolve(type);
                }
                return _containter.Resolve(type);
            }
            catch (ComponentNotRegisteredException e)
            {
                Debug.WriteLine("~~!~~~~!~~~~!~~~~!~~~~!~~~~!~~");
                Debug.WriteLine("Component not registered! \n" + e.Message);
                Debug.WriteLine("~~!~~~~!~~~~!~~~~!~~~~!~~~~!~~");
                throw;
            }
            catch (DependencyResolutionException e)
            {
                throw;
            }
        }

        public void BeginLifetimeScope(Action<IServiceLocator> scopedServiceLocator)
        {
            using (var scope = _containter.BeginLifetimeScope())
            using (var locator = scope.Resolve<IServiceLocator>() as ServiceLocator)
            {
                scopedServiceLocator?.Invoke(locator);
            }
        }

        public async Task BeginLifetimeScopeAsync(Func<IServiceLocator, Task> scopedServiceLocator)
        {
            using (var scope = _containter.BeginLifetimeScope())
            using (var locator = scope.Resolve<IServiceLocator>() as ServiceLocator)
            {
                await scopedServiceLocator?.Invoke(locator);
            }
        }

        public void Dispose()
        {
            _containter?.Dispose();
            _lifetimeScope?.Dispose();
        }
    }
}
