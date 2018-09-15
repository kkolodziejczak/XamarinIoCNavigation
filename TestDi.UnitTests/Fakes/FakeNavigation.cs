using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestDi.UnitTests.Fakes
{
    public class FakeNavigation : INavigation
    {
        public List<Page> _modalList = new List<Page>();
        public List<Page> _navigationList = new List<Page>();

        public IReadOnlyList<Page> ModalStack => _modalList;

        public IReadOnlyList<Page> NavigationStack => _navigationList;

        public void InsertPageBefore(Page page, Page before)
        {
            var index = _navigationList.IndexOf(before);
            _navigationList.Insert(index, page);
        }

        public Task<Page> PopAsync()
            => PopAsync(false);

        public Task<Page> PopAsync(bool animated)
        {
            return Task.Run(() =>
            {
                var pageToRemove = _navigationList.Last();
                _navigationList.Remove(pageToRemove);
                return pageToRemove;
            });
        }

        public Task<Page> PopModalAsync()
            => PopModalAsync(false);


        public Task<Page> PopModalAsync(bool animated)
        {
            return Task.Run(() =>
            {
                var pageToRemove = _modalList.Last();
                _modalList.Remove(pageToRemove);
                return pageToRemove;
            });
        }

        public Task PopToRootAsync()
            => PopToRootAsync(false);

        public Task PopToRootAsync(bool animated)
        {
            return Task.Run(() =>
            {
                for (var i = 1; i < _navigationList.Count; i++)
                {
                    _navigationList.RemoveAt(i);
                }
            });
        }

        public Task PushAsync(Page page)
            => PushAsync(page, false);

        public Task PushAsync(Page page, bool animated)
        {
            return Task.Run(() =>
            {
                _navigationList.Add(page);
            });
        }

        public Task PushModalAsync(Page page)
            => PushModalAsync(page, false);

        public Task PushModalAsync(Page page, bool animated)
        {
            return Task.Run(() =>
            {
                _modalList.Add(page);
            });
        }

        public void RemovePage(Page page)
        {
            _navigationList.Remove(page);
        }
    }


}
