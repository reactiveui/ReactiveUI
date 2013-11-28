using System;
using System.Linq;
using System.Windows.Forms;
using ReactiveUI.Winforms;
using Xunit;

namespace ReactiveUI.Tests.Winforms
{
    public class ViewModelViewHostTests
    {
        [Fact]
        public void SettingViewModelShouldAddTheViewtoItsControls()
        {
            var viewLocator = new FakeViewLocator { LocatorFunc = t => new FakeWinformsView() };
            var target = new ViewModelViewHost();
            target.ViewLocator = viewLocator;

            target.ViewModel = new FakeWinformViewModel();

            Assert.IsType<FakeWinformsView>(target.CurrentView);
            Assert.Equal(1, target.Controls.OfType<FakeWinformsView>().Count());
        }

        [Fact]
        public void ShouldDisposePreviousView()
        {
            var viewLocator = new FakeViewLocator { LocatorFunc = t => new FakeWinformsView() };
            var target = new ViewModelViewHost();
            target.ViewLocator = viewLocator;

            target.ViewModel = new FakeWinformViewModel();

            Control currentView = target.CurrentView;
            bool isDisposed = false;
            currentView.Disposed += (o, e) => isDisposed = true;

            // switch the viewmodel
            target.ViewModel = new FakeWinformViewModel();

            Assert.True(isDisposed);
        }

        [Fact]
        public void ShouldSetDefaultContentWhenViewModelIsNull()
        {
            var viewLocator = new FakeViewLocator { LocatorFunc = t => new FakeWinformsView() };
            var defaultContent = new Control();
            var target = new ViewModelViewHost { DefaultContent = defaultContent, ViewLocator = viewLocator };

            Assert.Null(target.CurrentView);
            Assert.True(target.Controls.Contains(defaultContent));
        }
    }

    class FakeViewLocator : IViewLocator
    {
        public Func<Type, IViewFor> LocatorFunc { get; set; }

        public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class
        {
            return this.LocatorFunc(viewModel.GetType());
        }
    }
}
