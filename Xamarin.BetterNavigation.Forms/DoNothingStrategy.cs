using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.Forms
{
    public class DoNothingStrategy : IPushStrategy, IPopStrategy
    {
        public Task BeforePopAsync(Page pageToPop)
        {
            return Task.CompletedTask;
        }

        public Task BeforePushAsync(Page pageToPush)
        {
            return Task.CompletedTask;
        }
    }

}
