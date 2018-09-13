using System.Threading.Tasks;

namespace TestDI.Interfaces
{
    public interface IAsyncInitialization
    {
        Task Initialization { get; }
    }
}
