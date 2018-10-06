using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using ReactiveUI.Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering;
using Avalonia.Platform;
using Avalonia;
using DynamicData;
using Xunit;

namespace ReactiveUI.Tests.Platforms.Avalonia
{
    public class AvaloniaActivationForViewFetcherTest
    {
        public class TestUserControl : UserControl, IActivatable { }

        public class FakeRenderDecorator : Decorator, IRenderRoot
        {
            public Size ClientSize => new Size(100, 100);

            public IRenderer Renderer { get; }

            public double RenderScaling => 1;

            public IRenderTarget CreateRenderTarget() => null;

            public void Invalidate(Rect rect) { }

            public Point PointToClient(Point point) => point;

            public Point PointToScreen(Point point) => point;
        }

        public class TestUserControlWithWhenActivated : UserControl, IActivatable
        {
            public bool Active { get; private set; }

            public TestUserControlWithWhenActivated()
            {
                this.WhenActivated(disposables => {
                    Active = true;
                    Disposable
                        .Create(() => Active = false)
                        .DisposeWith(disposables);
                });
            }
        }

        [Fact]
        public void VisualElementIsActivatedAndDeactivated()
        {
            var userControl = new TestUserControl();
            var activationForViewFetcher = new AvaloniaActivationForViewFetcher();

            activationForViewFetcher
                .GetActivationForView(userControl)
                .ToObservableChangeSet(scheduler: ImmediateScheduler.Instance)
                .Bind(out var activated)
                .Subscribe();

            var fakeRenderedDecorator = new FakeRenderDecorator();
            fakeRenderedDecorator.Child = userControl;
            new[] { true }.AssertAreEqual(activated);

            fakeRenderedDecorator.Child = null;
            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void ActivationForViewFetcherShouldSupportWhenActivated()
        {
            var userControl = new TestUserControlWithWhenActivated();
            Assert.False(userControl.Active);

            var fakeRenderedDecorator = new FakeRenderDecorator();
            fakeRenderedDecorator.Child = userControl;
            Assert.True(userControl.Active);

            fakeRenderedDecorator.Child = null;
            Assert.False(userControl.Active);
        }
    }
}
