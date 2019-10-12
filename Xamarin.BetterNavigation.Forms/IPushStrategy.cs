using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{

    public interface IPushStrategy
    {
        Task BeforePushAsync(Page pageToPush);
    }

}
