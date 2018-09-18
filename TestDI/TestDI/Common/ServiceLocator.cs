using System;
using System.Diagnostics;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;

namespace TestDI.Common
{
    public class ServiceLocator : IServiceLocator
    {
        private readonly ContainerBuilder _builder;
        private readonly IContainer _containter;

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

    }
}
