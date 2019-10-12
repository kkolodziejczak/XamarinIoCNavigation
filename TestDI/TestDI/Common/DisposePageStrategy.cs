using System;
using System.Threading.Tasks;
using Xamarin.BetterNavigation.Forms;
using Xamarin.Forms;

namespace TestDI.Common
{
    public class DisposePageStrategy : IPopStrategy
    {
        public async Task BeforePopAsync(Page pageToPop)
        {
            if (pageToPop is IDisposable disposablePage)
            {
                disposablePage.Dispose();
            }
        }
    }
}
