using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.BetterNavigation.Forms;
using Xamarin.Forms;

namespace Xamarin.BetterNavigation.UnitTests.Fakes
{
    internal class RecordingStrategy : IPopStrategy, IPushStrategy
    {
        private StringBuilder _stringBuilder;

        private readonly IPageLocator _pageLocator;

        public string Events => _stringBuilder.ToString();

        public RecordingStrategy(IPageLocator pageLocator)
        {
            Reset();
            _pageLocator = pageLocator;
        }

        public Task BeforePopAsync(Page pageToPop)
        {
            _stringBuilder.Append($"POP{_pageLocator.GetPageName(pageToPop)}");
            return Task.CompletedTask;
        }

        public void Reset()
        {
            _stringBuilder = new StringBuilder();
        }

        public Task BeforePushAsync(Page pageToPush)
        {
            _stringBuilder.Append($"PUSH{_pageLocator.GetPageName(pageToPush)}");
            return Task.CompletedTask;
        }
    }
}
