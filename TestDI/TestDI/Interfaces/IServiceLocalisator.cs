using System;

namespace TestDI.Interfaces
{
    public interface IServiceLocalisator
    {
        T Get<T>();
        object Get(Type type);
    }
}
