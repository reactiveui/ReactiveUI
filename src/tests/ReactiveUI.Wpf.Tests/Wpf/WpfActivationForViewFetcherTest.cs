// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;

using DynamicData;

using ReactiveUI.Builder;
using ReactiveUI.Tests.Wpf.Mocks;
using Splat;

namespace ReactiveUI.Tests.Wpf;

[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WpfActivationForViewFetcherTest
{
    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task GetIsDesignModeReturnsFalseForRuntimeWpfView()
    {
        var view = new RuntimeActivatableViewFor();

        await Assert.That(view.GetIsDesignMode()).IsFalse();
    }

    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task GetIsDesignModeReturnsTrueForDesignerWpfView()
    {
        var view = new DesignModeActivatableViewFor();

        await Assert.That(view.GetIsDesignMode()).IsTrue();
    }

    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task WhenActivatedInDesignerModeWithoutRegisteredFetcherDoesNotThrow()
    {
        using var locator = new ModernDependencyResolver();
        using (locator.WithResolver())
        {
            ViewForMixins.ResetActivationFetcherCacheForTesting();

            var view = new DesignModeActivatableUserControl();

            await Assert.That(view.ActivationDisposable).IsNotNull();
            view.ActivationDisposable.Dispose();
        }

        ViewForMixins.ResetActivationFetcherCacheForTesting();
    }

    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task WpfWhenActivatedOverloadsShortCircuitInDesignerModeWithoutRegisteredFetcher()
    {
        using var locator = new ModernDependencyResolver();
        using (locator.WithResolver())
        {
            ViewForMixins.ResetActivationFetcherCacheForTesting();

            var view = new DesignModeActivatableViewFor();
            var disposables = new CompositeDisposable(
                view.WhenActivated(static () => [Disposable.Empty]),
                view.WhenActivated(static (Action<IDisposable> _) => { }),
                view.WhenActivated(static (CompositeDisposable _) => { }),
                view.WhenActivated(static () => [Disposable.Empty], view),
                view.WhenActivated(static (Action<IDisposable> _) => { }, view));

            await Assert.That(disposables.Count).IsEqualTo(5);
            disposables.Dispose();
        }

        ViewForMixins.ResetActivationFetcherCacheForTesting();
    }

    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task WpfWhenActivatedOverloadsDelegateToRuntimeActivationFetcher()
    {
        using var locator = new ModernDependencyResolver();
        locator
            .CreateReactiveUIBuilder()
            .WithWpf()
            .Build();

        using (locator.WithResolver())
        {
            ViewForMixins.ResetActivationFetcherCacheForTesting();

            var view = new RuntimeActivatableViewFor();
            var activationCount = 0;
            var disposables = new CompositeDisposable(
                view.WhenActivated(() =>
                {
                    activationCount++;
                    return [Disposable.Empty];
                }),
                view.WhenActivated((Action<IDisposable> _) => activationCount++),
                view.WhenActivated((CompositeDisposable _) => activationCount++),
                view.WhenActivated(
                    () =>
                    {
                        activationCount++;
                        return [Disposable.Empty];
                    },
                    view),
                view.WhenActivated(
                    (Action<IDisposable> _) => activationCount++,
                    view));

            view.RaiseEvent(new RoutedEventArgs
            {
                RoutedEvent = FrameworkElement.LoadedEvent
            });

            await Assert.That(activationCount).IsEqualTo(5);
            disposables.Dispose();
        }

        ViewForMixins.ResetActivationFetcherCacheForTesting();
    }

    [Test]
    public async Task FrameworkElementIsActivatedAndDeactivated()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        await new[] { true }.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        await new[] { true, false }.AssertAreEqual(activated);
    }

    [Test]
    public async Task IsHitTestVisibleActivatesFrameworkElement()
    {
        var uc = new WpfTestUserControl
        {
            IsHitTestVisible = false
        };
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        // Loaded has happened.
        await new[] { true }.AssertAreEqual(activated);

        uc.IsHitTestVisible = true;

        // IsHitTestVisible true, we don't want the event to repeat unnecessarily.
        await new[] { true }.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        // We had both a loaded/hit test visible change/unloaded happen.
        await new[] { true, false }.AssertAreEqual(activated);
    }

    [Test]
    public async Task IsHitTestVisibleDeactivatesFrameworkElement()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        await new[] { true }.AssertAreEqual(activated);

        uc.IsHitTestVisible = false;

        await new[] { true, false }.AssertAreEqual(activated);
    }

    [Test]
    public async Task FrameworkElementIsActivatedAndDeactivatedWithHitTest()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        await new[] { true }.AssertAreEqual(activated);

        // this should deactivate it
        uc.IsHitTestVisible = false;

        await new[] { true, false }.AssertAreEqual(activated);

        // this should activate it
        uc.IsHitTestVisible = true;

        await new[] { true, false, true }.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        await new[] { true, false, true, false }.AssertAreEqual(activated);
    }

    private sealed class DesignModeActivatableUserControl : UserControl, IActivatableView
    {
        static DesignModeActivatableUserControl() =>
            DesignerProperties.IsInDesignModeProperty.OverrideMetadata(
                typeof(DesignModeActivatableUserControl),
                new FrameworkPropertyMetadata(true));

        public DesignModeActivatableUserControl() => ActivationDisposable = this.WhenActivated(static _ => { });

        public IDisposable ActivationDisposable { get; }
    }

    private sealed class RuntimeActivatableViewFor : ReactiveUserControl<object>;

    private sealed class DesignModeActivatableViewFor : UserControl, IViewFor<object>
    {
        static DesignModeActivatableViewFor() =>
            DesignerProperties.IsInDesignModeProperty.OverrideMetadata(
                typeof(DesignModeActivatableViewFor),
                new FrameworkPropertyMetadata(true));

        public object? ViewModel { get; set; }
    }
}
