using System.Linq;
using System.Windows.Forms;
using ReactiveUI.Winforms;
using Xunit;

namespace ReactiveUI.Tests.Winforms
{
    public class RoutedViewHostTests
    {
        [Fact]
        public void ShouldDisposePreviousView()
        {
            var viewLocator = new FakeViewLocator { LocatorFunc = t => new FakeWinformsView() };
            var router = new RoutingState();
            var target = new RoutedViewHost { Router = router, ViewLocator = viewLocator };
            router.Navigate.Execute(new FakeWinformViewModel());

            FakeWinformsView currentView = target.Controls.OfType<FakeWinformsView>().Single();
            bool isDisposed = false;
            currentView.Disposed += (o, e) => isDisposed = true;

            // switch the viewmodel
            router.Navigate.Execute(new FakeWinformViewModel());

            Assert.True(isDisposed);
        }

        [Fact]
        public void ShouldSetDefaultContentWhenViewModelIsNull()
        {
            var defaultContent = new Control();
            var viewLocator = new FakeViewLocator { LocatorFunc = t => new FakeWinformsView() };
            var router = new RoutingState();
            var target = new RoutedViewHost
            {
                Router = router,
                ViewLocator = viewLocator,
                DefaultContent = defaultContent
            };

            Assert.True(target.Controls.Contains(defaultContent));
        }

        [Fact]
        public void WhenRoutedToViewModelItShouldAddViewToControls()
        {
            var viewLocator = new FakeViewLocator { LocatorFunc = t => new FakeWinformsView() };
            var router = new RoutingState();
            var target = new RoutedViewHost { Router = router, ViewLocator = viewLocator };
            router.Navigate.Execute(new FakeWinformViewModel());

            Assert.Equal(1, target.Controls.OfType<FakeWinformsView>().Count());
        }
    }
}
