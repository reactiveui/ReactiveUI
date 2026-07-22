// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using ReactiveUI.Tests.Utilities;
using ReactiveUI.Tests.Wpf.Mocks;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for the WPF activation-for-view fetcher.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WpfActivationForViewFetcherTest
{
    /// <summary>The number of <c>WhenActivated</c> overloads exercised by the overload tests.</summary>
    private const int WhenActivatedOverloadCount = 5;

    /// <summary>The expected activation sequence for a single activation.</summary>
    private static readonly bool[] _expectedActivated = [true];

    /// <summary>The expected activation sequence for an activation followed by a deactivation.</summary>
    private static readonly bool[] _expectedActivatedDeactivated = [true, false];

    /// <summary>The expected activation sequence for activate, deactivate, then activate.</summary>
    private static readonly bool[] _expectedActivatedDeactivatedActivated = [true, false, true];

    /// <summary>The expected activation sequence for activate, deactivate, activate, then deactivate.</summary>
    private static readonly bool[] _expectedActivatedDeactivatedActivatedDeactivated = [true, false, true, false];

    /// <summary>Verifies a framework element is activated on load and deactivated on unload.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task GetIsDesignModeReturnsFalseForRuntimeWpfView()
    {
        var view = new RuntimeActivatableViewFor();

        await Assert.That(view.GetIsDesignMode()).IsFalse();
    }

    /// <summary>Verifies that a designer WPF view reports design mode as true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task GetIsDesignModeReturnsTrueForDesignerWpfView()
    {
        var view = new DesignModeActivatableViewFor();

        await Assert.That(view.GetIsDesignMode()).IsTrue();
    }

    /// <summary>Verifies that activating a view in designer mode without a registered fetcher does not throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that the <c>WhenActivated</c> overloads short-circuit in designer mode without a registered fetcher.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task WpfWhenActivatedOverloadsShortCircuitInDesignerModeWithoutRegisteredFetcher()
    {
        using var locator = new ModernDependencyResolver();
        using (locator.WithResolver())
        {
            ViewForMixins.ResetActivationFetcherCacheForTesting();

            var view = new DesignModeActivatableViewFor();
            var disposables = new MultipleDisposable(
                view.WhenActivated(static () => [Scope.Empty]),
                view.WhenActivated(static (Action<IDisposable> _) => { }),
                view.WhenActivated(static (MultipleDisposable _) => { }),
                view.WhenActivated(static () => [Scope.Empty], view),
                view.WhenActivated(static (Action<IDisposable> _) => { }, view));

            await Assert.That(disposables.Count).IsEqualTo(WhenActivatedOverloadCount);
            disposables.Dispose();
        }

        ViewForMixins.ResetActivationFetcherCacheForTesting();
    }

    /// <summary>Verifies that the <c>WhenActivated</c> overloads delegate to the runtime activation fetcher.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WpfTestExecutor>]
    public async Task WpfWhenActivatedOverloadsDelegateToRuntimeActivationFetcher()
    {
        using var locator = new ModernDependencyResolver();
        _ = locator
            .CreateReactiveUIBuilder()
            .WithWpf()
            .Build();

        using (locator.WithResolver())
        {
            ViewForMixins.ResetActivationFetcherCacheForTesting();

            var view = new RuntimeActivatableViewFor();
            var activationCount = 0;
            var disposables = new MultipleDisposable(
                view.WhenActivated(() =>
                {
                    activationCount++;
                    return [Scope.Empty];
                }),
                view.WhenActivated((Action<IDisposable> _) => activationCount++),
                view.WhenActivated((MultipleDisposable _) => activationCount++),
                view.WhenActivated(
                    () =>
                    {
                        activationCount++;
                        return [Scope.Empty];
                    },
                    view),
                view.WhenActivated(
                    (Action<IDisposable> _) => activationCount++,
                    view));

            view.RaiseEvent(new()
            {
                RoutedEvent = FrameworkElement.LoadedEvent
            });

            await Assert.That(activationCount).IsEqualTo(WhenActivatedOverloadCount);
            disposables.Dispose();
        }

        ViewForMixins.ResetActivationFetcherCacheForTesting();
    }

    /// <summary>Verifies a framework element raises activation on load and deactivation on unload.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task FrameworkElementIsActivatedAndDeactivated()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        var activated = obs.Collect();

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

    /// <summary>Verifies that making a loaded element hit-test visible does not re-raise activation.</summary>
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
        var activated = obs.Collect();

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

    /// <summary>Verifies that clearing hit-test visibility deactivates a loaded element.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsHitTestVisibleDeactivatesFrameworkElement()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        var activated = obs.Collect();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        await _expectedActivated.AssertAreEqual(activated);

        uc.IsHitTestVisible = false;

        await _expectedActivatedDeactivated.AssertAreEqual(activated);
    }

    /// <summary>Verifies activation and deactivation toggle correctly as hit-test visibility changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task FrameworkElementIsActivatedAndDeactivatedWithHitTest()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        var activated = obs.Collect();

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

    /// <summary>A user control that reports itself as being in design mode.</summary>
    private sealed class DesignModeActivatableUserControl : UserControl, IActivatableView
    {
        /// <summary>Initializes static members of the <see cref="DesignModeActivatableUserControl"/> class.</summary>
        static DesignModeActivatableUserControl() =>
            DesignerProperties.IsInDesignModeProperty.OverrideMetadata(
                typeof(DesignModeActivatableUserControl),
                new FrameworkPropertyMetadata(true));

        /// <summary>Initializes a new instance of the <see cref="DesignModeActivatableUserControl"/> class.</summary>
        public DesignModeActivatableUserControl() => ActivationDisposable = this.WhenActivated(static (MultipleDisposable _) => { });

        /// <summary>Gets the disposable returned by the activation subscription.</summary>
        public IDisposable ActivationDisposable { get; }
    }

    /// <summary>A reactive user control used to test runtime activation.</summary>
    private sealed class RuntimeActivatableViewFor : ReactiveUserControl<object>;

    /// <summary>A view that reports itself as being in design mode.</summary>
    private sealed class DesignModeActivatableViewFor : UserControl, IViewFor<object>
    {
        /// <summary>Initializes static members of the <see cref="DesignModeActivatableViewFor"/> class.</summary>
        static DesignModeActivatableViewFor() =>
            DesignerProperties.IsInDesignModeProperty.OverrideMetadata(
                typeof(DesignModeActivatableViewFor),
                new FrameworkPropertyMetadata(true));

        /// <summary>Gets or sets the view model.</summary>
        public object? ViewModel { get; set; }
    }
}
