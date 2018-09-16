using System;

namespace TestDI
{
    public interface IServiceLocator
    {
        T Get<T>();
        object Get(Type type);
    }
}
