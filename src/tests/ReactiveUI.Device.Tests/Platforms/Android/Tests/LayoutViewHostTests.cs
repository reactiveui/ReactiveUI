// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="LayoutViewHost"/> and the view-host lookup in <see cref="ViewMixins"/>.</summary>
public class LayoutViewHostTests
{
    /// <summary>The bind constructor inflates the layout and hands the inflated view to the callback.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task BindConstructor_PassesTheInflatedViewToTheCallback()
    {
        var host = await MainThread.RunAsync(static () =>
            new BoundHost(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), false));

        await Assert.That(host.View).IsNotNull();
        await Assert.That(host.BoundView).IsSameReferenceAs(host.View);
        await Assert.That(host.View!.Id).IsEqualTo(Resource.Id.WireUpRoot);
        await Assert.That(host.TitleText).IsNotNull();
    }

    /// <summary>A view created by a host finds that host again through its tag.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetViewHost_ReturnsTheHostThatOwnsTheView()
    {
        var (host, found, typed) = await MainThread.RunAsync(static () =>
        {
            var created = new BoundHost(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), false);
            return (created, created.View!.GetViewHost(), created.View!.GetViewHost<BoundHost>());
        });

        await Assert.That(found).IsSameReferenceAs(host);
        await Assert.That(typed).IsSameReferenceAs(host);
    }

    /// <summary>A view no host owns has no view host.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetViewHost_OnAPlainView_ReturnsNull()
    {
        var found = await MainThread.RunAsync(static () => new TextView(ActivityLauncher.TargetContext).GetViewHost());

        await Assert.That(found).IsNull();
    }

    /// <summary>Attaching to the root adds the inflated layout to the parent and returns the parent as the view.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task AttachToRoot_AddsTheLayoutToTheParent()
    {
        var (parent, host) = await MainThread.RunAsync(static () =>
        {
            var frame = new FrameLayout(ActivityLauncher.TargetContext);
            return (frame, new BoundHost(ActivityLauncher.TargetContext, frame, true));
        });

        await Assert.That(parent.ChildCount).IsEqualTo(1);
        await Assert.That(host.View).IsSameReferenceAs(parent);
    }

    /// <summary>The conversion to <see cref="View"/> returns the host's view.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ImplicitConversion_ReturnsTheHostedView()
    {
        var host = await MainThread.RunAsync(static () =>
            new BoundHost(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), false));

        View? converted = host;

        await Assert.That(converted).IsSameReferenceAs(host.View);
        await Assert.That(host.ToView()).IsSameReferenceAs(host.View);
    }

    /// <summary>Assigning a new view re-tags it so the lookup finds the host from the new view.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SettingView_TagsTheNewViewWithTheHost()
    {
        var (host, found) = await MainThread.RunAsync(static () =>
        {
            var created = new BoundHost(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), false);
            var replacement = new TextView(ActivityLauncher.TargetContext);
            created.View = replacement;
            return (created, replacement.GetViewHost());
        });

        await Assert.That(found).IsSameReferenceAs(host);
    }
}
