// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
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

/// <summary>
/// Tests for the WPF activation-for-view fetcher.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WpfActivationForViewFetcherTest
{
    private static readonly bool[] _expectedActivated = [true];
    private static readonly bool[] _expectedActivatedDeactivated = [true, false];
    private static readonly bool[] _expectedActivatedDeactivatedActivated = [true, false, true];
    private static readonly bool[] _expectedActivatedDeactivatedActivatedDeactivated = [true, false, true, false];

    /// <summary>
    /// Verifies a framework element is activated on load and deactivated on unload.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

        await _expectedActivated.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        await _expectedActivatedDeactivated.AssertAreEqual(activated);
    }

    /// <summary>
    /// Verifies that making a loaded element hit-test visible does not re-raise activation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        await _expectedActivated.AssertAreEqual(activated);

        uc.IsHitTestVisible = true;

        // IsHitTestVisible true, we don't want the event to repeat unnecessarily.
        await _expectedActivated.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        // We had both a loaded/hit test visible change/unloaded happen.
        await _expectedActivatedDeactivated.AssertAreEqual(activated);
    }

    /// <summary>
    /// Verifies that clearing hit-test visibility deactivates a loaded element.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

        await _expectedActivated.AssertAreEqual(activated);

        uc.IsHitTestVisible = false;

        await _expectedActivatedDeactivated.AssertAreEqual(activated);
    }

    /// <summary>
    /// Verifies activation and deactivation toggle correctly as hit-test visibility changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

        await _expectedActivated.AssertAreEqual(activated);

        // this should deactivate it
        uc.IsHitTestVisible = false;

        await _expectedActivatedDeactivated.AssertAreEqual(activated);

        // this should activate it
        uc.IsHitTestVisible = true;

        await _expectedActivatedDeactivatedActivated.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        await _expectedActivatedDeactivatedActivatedDeactivated.AssertAreEqual(activated);
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
