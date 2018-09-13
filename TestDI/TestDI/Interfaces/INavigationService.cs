using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDI.Interfaces
{
    public interface INavigationService
    {
        /// <exception cref="KeyNotFoundException"/>
        T NavigationParameters<T>(string parameterKey);
        Task PopPageToRootAsync();
        Task PopPageAsync(byte pagesToPop = 1);
        Task GoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters);
    }
}
