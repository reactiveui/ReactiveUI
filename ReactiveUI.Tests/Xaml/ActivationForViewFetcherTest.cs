using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ActivationForViewFetcherTest
    {
        public class TestUserControl : UserControl, IActivatable
        {
            public TestUserControl()
            {

            }
        }

        [Fact]
        public void FrameworkElementIsActivatedAndDeactivated()
        {
            var uc = new TestUserControl();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            var activated = obs.CreateCollection();

            RoutedEventArgs loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            RoutedEventArgs unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void IsHitTestVisibleActivatesFrameworkElement()
        {
            var uc = new TestUserControl();
            uc.IsHitTestVisible = false;
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            var activated = obs.CreateCollection();

            RoutedEventArgs loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            // IsHitTestVisible still false
            new bool[0].AssertAreEqual(activated);

            uc.IsHitTestVisible = true;

            // IsHitTestVisible true
            new[] { true }.AssertAreEqual(activated);

            RoutedEventArgs unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void IsHitTestVisibleDeactivatesFrameworkElement()
        {
            var uc = new TestUserControl();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            var activated = obs.CreateCollection();

            RoutedEventArgs loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            uc.IsHitTestVisible = false;

            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void FrameworkElementIsActivatedAndDeactivatedWithHitTest()
        {
            var uc = new TestUserControl();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            var activated = obs.CreateCollection();

            RoutedEventArgs loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            // this should deactivate it
            uc.IsHitTestVisible = false;

            new[] { true, false }.AssertAreEqual(activated);

            // this should activate it
            uc.IsHitTestVisible = true;

            new[] { true, false, true }.AssertAreEqual(activated);

            RoutedEventArgs unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false, true, false }.AssertAreEqual(activated);
        }
    }
}
