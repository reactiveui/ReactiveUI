// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Android.Widget;
using static ReactiveUI.ControlFetcherMixins;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="ReactiveViewHost{TViewModel}"/> over a real inflated layout.</summary>
public class ReactiveViewHostTests
{
    /// <summary>Setting the view model raises <see cref="INotifyPropertyChanged.PropertyChanged"/> and the <c>Changed</c> stream.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SettingViewModel_RaisesPropertyChangedAndChanged()
    {
        var host = await MainThread.RunAsync(static () => CreateHost());
        var viewModel = new TestViewModel("first");
        List<string?> classic = [];
        host.PropertyChanged += (_, e) => classic.Add(e.PropertyName);

        var changed = host.Changed.FirstValueAsync(out var subscription);
        using (subscription)
        {
            host.ViewModel = viewModel;

            var change = await changed.WithTimeout("the Changed notification");
            await Assert.That(change.PropertyName).IsEqualTo(nameof(TestViewHost.ViewModel));
        }

        await Assert.That(classic).Contains(nameof(TestViewHost.ViewModel));
        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Setting the view model through <see cref="IViewFor"/> reaches the typed property.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SettingViewModelThroughIViewFor_SetsTheTypedViewModel()
    {
        var host = await MainThread.RunAsync(static () => CreateHost());
        var viewModel = new TestViewModel("typed");

        ((IViewFor)host).ViewModel = viewModel;

        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(((IViewFor)host).ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Suppressing change notifications silences the host until the suppression is disposed.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SuppressChangeNotifications_SilencesTheHost()
    {
        var host = await MainThread.RunAsync(static () => CreateHost());
        var raised = 0;
        host.PropertyChanged += (_, _) => raised++;

        using (host.SuppressChangeNotifications())
        {
            await Assert.That(host.AreChangeNotificationsEnabled()).IsFalse();
            host.ViewModel = new("silent");
        }

        await Assert.That(raised).IsEqualTo(0);
        await Assert.That(host.AreChangeNotificationsEnabled()).IsTrue();
    }

    /// <summary>The bind-callback constructor wires the control the callback finds and leaves reflection metadata unset.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task BindConstructor_WiresThroughTheCallback()
    {
        var host = await MainThread.RunAsync(static () =>
            new TestViewHost(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), bindTitle: true));

        await Assert.That(host.BoundByCallback).IsTrue();
        await Assert.That(host.TitleText).IsNotNull();
        await Assert.That(host.HasLegacyPropertyMetadata).IsFalse();
    }

    /// <summary>The Unsafe host's constructor wires controls by reflection and prepares the legacy property metadata.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task UnsafeConstructor_WiresByReflection()
    {
        var host = await MainThread.RunAsync(static () =>
            new UnsafeTestViewHost(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), ResolveStrategy.Implicit));

        await Assert.That(host.TitleText).IsNotNull();
        await Assert.That(host.TitleText!.Id).IsEqualTo(Resource.Id.TitleText);
        await Assert.That(host.HasLegacyPropertyMetadata).IsTrue();
    }

    /// <summary>The host's view finds the typed view host, as adapters do when they bind a row.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetViewHost_FindsTheTypedViewFor()
    {
        var (host, found) = await MainThread.RunAsync(static () =>
        {
            var created = CreateHost();
            return (created, created.View!.GetViewHost() as IViewFor<TestViewModel>);
        });

        await Assert.That(found).IsSameReferenceAs(host);
    }

    /// <summary>Creates a host without wire-up against a detached parent.</summary>
    /// <returns>The host.</returns>
    private static TestViewHost CreateHost() =>
        new(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext));
}
