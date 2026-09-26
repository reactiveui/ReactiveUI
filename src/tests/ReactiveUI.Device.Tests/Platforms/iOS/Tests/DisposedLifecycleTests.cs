// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests that the reactive UIKit types ignore lifecycle callbacks that arrive after they are disposed.</summary>
/// <remarks>
/// <para>
/// UIKit keeps its own reference to a controller or view, so it can call one back after the app has disposed the
/// managed object. It runs <c>viewDidDisappear:</c> from a block it defers until after a Core Animation commit, for
/// example. A managed exception thrown from such a callback terminates the app.
/// </para>
/// <para>
/// Each test keeps the native object alive while it calls the disposed object, the way UIKit does. It then lets UIKit
/// run its own callbacks: a failure there terminates the app rather than failing the test.
/// </para>
/// </remarks>
[NotInParallel(UIKitHarness.WindowKey)]
public class DisposedLifecycleTests
{
    /// <summary>A disposed <see cref="ReactiveViewController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ViewController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new TestViewController());

    /// <summary>A disposed <see cref="ReactiveTableViewController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task TableViewController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.TableViewController());

    /// <summary>A disposed <see cref="ReactiveCollectionViewController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task CollectionViewController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.CollectionViewController());

    /// <summary>A disposed <see cref="ReactivePageViewController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task PageViewController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.PageViewController());

    /// <summary>A disposed <see cref="ReactiveNavigationController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task NavigationController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.NavigationController());

    /// <summary>A disposed <see cref="ReactiveTabBarController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task TabBarController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.TabBarController());

    /// <summary>A disposed <see cref="ReactiveSplitViewController"/> ignores appearance callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task SplitViewController_IgnoresCallbacksAfterDispose() =>
        AssertControllerIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.SplitViewController());

    /// <summary>A disposed <see cref="ReactiveView"/> ignores superview callbacks and a pending forced activation.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task View_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.View());

    /// <summary>A disposed <see cref="ReactiveControl"/> ignores superview callbacks and a pending forced activation.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task Control_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.Control());

    /// <summary>A disposed <see cref="ReactiveImageView"/> ignores superview callbacks and a pending forced activation.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ImageView_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.ImageView());

    /// <summary>A disposed <see cref="ReactiveTableView"/> ignores superview callbacks and a pending forced activation.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task TableView_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.TableView());

    /// <summary>A disposed <see cref="ReactiveTableViewCell"/> ignores superview callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task TableViewCell_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.TableViewCell());

    /// <summary>A disposed <see cref="ReactiveCollectionView"/> ignores superview callbacks and a pending forced activation.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task CollectionView_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.CollectionView());

    /// <summary>A disposed <see cref="ReactiveCollectionViewCell"/> ignores superview callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task CollectionViewCell_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.CollectionViewCell());

    /// <summary>A disposed <see cref="ReactiveCollectionReusableView"/> ignores superview callbacks.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task CollectionReusableView_IgnoresCallbacksAfterDispose() =>
        AssertViewIgnoresLateCallbacks(static () => new ReactiveUIKitTypes.CollectionReusableView());

    /// <summary>
    /// Shows a controller as the window's root, disposes it, and calls its appearance callbacks directly. Then it
    /// replaces the root, so UIKit sends the disposed controller its own disappear callbacks.
    /// </summary>
    /// <param name="create">Creates the controller on the main thread.</param>
    /// <returns>A task representing the check.</returns>
    private static async Task AssertControllerIgnoresLateCallbacks(Func<UIViewController> create)
    {
        var controller = await MainThread.RunAsync(create);
        await UIKitHarness.ShowAsync(controller);
        try
        {
            // The window still holds the native controller, so it stays alive after Dispose, as it does for UIKit.
            await Assert.That(() => MainThread.RunAsync(() =>
            {
                controller.Dispose();
                controller.ViewWillAppear(false);
                controller.ViewDidAppear(false);
                controller.ViewWillDisappear(false);
                controller.ViewDidDisappear(false);
            })).ThrowsNothing();
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }

        await UIKitHarness.SettleAsync();
    }

    /// <summary>
    /// Adds a view to a container, forces an activation that runs later, and disposes the view. Then it calls the
    /// view's superview callbacks directly and removes it, so UIKit sends the disposed view its own callbacks.
    /// </summary>
    /// <param name="create">Creates the view on the main thread.</param>
    /// <returns>A task representing the check.</returns>
    private static async Task AssertViewIgnoresLateCallbacks(Func<UIView> create)
    {
        await Assert.That(() => MainThread.RunAsync(() =>
        {
            using var container = new UIView();
            var view = create();

            // The container holds the native view, so it stays alive after Dispose, as it does for UIKit.
            container.AddSubview(view);
            if (view is ICanForceManualActivation manual)
            {
                manual.Activate(true);
            }

            view.Dispose();
            view.WillMoveToSuperview(null);
            view.WillMoveToSuperview(container);
            view.RemoveFromSuperview();
        })).ThrowsNothing();

        // Runs the forced activation, which was queued before the view was disposed.
        await UIKitHarness.SettleAsync();
    }
}
