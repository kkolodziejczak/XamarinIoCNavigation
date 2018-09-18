using System;

namespace TestDI.Common
{
    public interface IServiceLocator
    {
        T Get<T>();
        object Get(Type type);
    }
}
