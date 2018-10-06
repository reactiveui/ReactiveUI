using System;
using System.Reactive.Concurrency;
using Avalonia.Controls;
using ReactiveUI.Avalonia;
using DynamicData;
using Avalonia;
using Avalonia.Rendering;
using Avalonia.Platform;
using Xunit;

namespace ReactiveUI.Tests.Platforms.Avalonia
{
    public class AvaloniaActivationForViewFetcherTest
    {
        public class TestUserControl : UserControl, IActivatable
        {
            public TestUserControl() { }
        }

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
    }
}
