using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{
    public interface IPopStrategy
    {
        Task BeforePopAsync(Page pageToPop);
    }
}
