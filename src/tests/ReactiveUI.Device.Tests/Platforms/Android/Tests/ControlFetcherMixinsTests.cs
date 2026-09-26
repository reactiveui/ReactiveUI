// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Android.Widget;
using static ReactiveUI.ControlFetcherMixins;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests the Android control wire-up in <see cref="ControlFetcherMixins"/> against real inflated layouts.</summary>
public class ControlFetcherMixinsTests
{
    /// <summary>Gets the assembly whose resource designer holds the test layouts' ids.</summary>
    private static Assembly TestAssembly => typeof(ControlFetcherMixinsTests).Assembly;

    /// <summary>Wiring a view finds each child control by its property name.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task WireUpControls_OnView_WiresChildrenByPropertyName()
    {
        var (title, save) = await MainThread.RunAsync(static () =>
        {
            var layout = WiredLayout.Create(ActivityLauncher.TargetContext);
            layout.WireUpControls();
            return (layout.TitleText, layout.SaveButton);
        });

        await Assert.That(title).IsNotNull();
        await Assert.That(title!.Id).IsEqualTo(Resource.Id.TitleText);
        await Assert.That(save).IsNotNull();
        await Assert.That(save!.Id).IsEqualTo(Resource.Id.SaveButton);
    }

    /// <summary>Resolving a control by name returns the inflated child with that id.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetControl_ReturnsTheChildWithTheNamedId()
    {
        var control = await MainThread.RunAsync(static () =>
            WiredLayout.Create(ActivityLauncher.TargetContext).GetControl(TestAssembly, "SaveButton"));

        await Assert.That(control).IsTypeOf<Button>();
    }

    /// <summary>Resource names are matched without regard to case.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetControl_MatchesTheResourceNameIgnoringCase()
    {
        var control = await MainThread.RunAsync(static () =>
            WiredLayout.Create(ActivityLauncher.TargetContext).GetControl(TestAssembly, "RENAMED_INPUT"));

        await Assert.That(control).IsTypeOf<EditText>();
    }

    /// <summary>A second lookup on the same root returns the cached control.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetControl_ReturnsTheSameInstanceForTheSameRoot()
    {
        var (first, second) = await MainThread.RunAsync(static () =>
        {
            var layout = WiredLayout.Create(ActivityLauncher.TargetContext);
            return (layout.GetControl(TestAssembly, "TitleText"), layout.GetControl(TestAssembly, "TitleText"));
        });

        await Assert.That(second).IsSameReferenceAs(first);
    }

    /// <summary>A name with no resource id fails with <see cref="MissingFieldException"/>.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetControl_WithAnUnknownName_ThrowsMissingFieldException()
    {
        var error = await MainThread.RunAsync(static () => Catch(static () =>
            WiredLayout.Create(ActivityLauncher.TargetContext).GetControl(TestAssembly, "NoSuchControl")));

        await Assert.That(error).IsTypeOf<MissingFieldException>();
    }

    /// <summary>The opt-in strategy wires only the properties marked with <see cref="WireUpResourceAttribute"/>.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task LayoutViewHost_ExplicitOptIn_WiresOnlyMarkedProperties()
    {
        var host = await MainThread.RunAsync(static () => CreateHost(ResolveStrategy.ExplicitOptIn));

        await Assert.That(host.TitleText).IsNull();
        await Assert.That(host.SaveButton).IsNotNull();
        await Assert.That(host.Input).IsNotNull();
        await Assert.That(host.Input!.Id).IsEqualTo(Resource.Id.renamed_input);
        await Assert.That(host.Ignored).IsNull();
    }

    /// <summary>The opt-out strategy wires every control property except those marked with <see cref="IgnoreResourceAttribute"/>.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task WireUpControls_ExplicitOptOut_SkipsIgnoredProperties()
    {
        var layout = await MainThread.RunAsync(static () =>
        {
            var wired = OptOutLayout.Create(ActivityLauncher.TargetContext);
            wired.WireUpControls(ResolveStrategy.ExplicitOptOut);
            return wired;
        });

        await Assert.That(layout.TitleText).IsNotNull();
        await Assert.That(layout.Input).IsNotNull();
        await Assert.That(layout.Input!.Id).IsEqualTo(Resource.Id.renamed_input);
        await Assert.That(layout.Ignored).IsNull();
    }

    /// <summary>The opt-out strategy on a layout host wires every control property except the ignored one, and keeps the host's own view.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task LayoutViewHost_ExplicitOptOut_SkipsIgnoredPropertiesAndKeepsTheHostView()
    {
        var host = await MainThread.RunAsync(static () => CreateHost(ResolveStrategy.ExplicitOptOut));

        await Assert.That(host.View).IsNotNull();
        await Assert.That(host.View!.Id).IsEqualTo(Resource.Id.WireUpRoot);
        await Assert.That(host.TitleText).IsNotNull();
        await Assert.That(host.SaveButton).IsNotNull();
        await Assert.That(host.Input).IsNotNull();
        await Assert.That(host.Input!.Id).IsEqualTo(Resource.Id.renamed_input);
        await Assert.That(host.Ignored).IsNull();
    }

    /// <summary>The implicit strategy does not honour <see cref="IgnoreResourceAttribute"/>, so a control with no matching id fails the wire-up.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task LayoutViewHost_Implicit_WithAnUnmatchedControl_ThrowsMissingFieldException()
    {
        var error = await MainThread.RunAsync(static () => Catch(static () => CreateHost(ResolveStrategy.Implicit)));

        await Assert.That(error).IsTypeOf<MissingFieldException>();
        await Assert.That(error!.Message).Contains(nameof(AutoWiredHost.Ignored));
    }

    /// <summary>An activity wires its content view's controls by property name.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task WireUpControls_OnActivity_WiresTheContentView()
    {
        var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();
        try
        {
            await Assert.That(activity.WireUpError).IsNull();
            await Assert.That(activity.TitleText).IsNotNull();
            await Assert.That(activity.TitleText!.Id).IsEqualTo(Resource.Id.TitleText);
            await Assert.That(activity.SaveButton).IsNotNull();
        }
        finally
        {
            await ActivityLauncher.FinishAsync(activity);
        }
    }

    /// <summary>An AndroidX fragment wires its inflated view, including a resource-name override.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task WireUpControls_OnAndroidXFragment_WiresTheInflatedView()
    {
        var activity = await ActivityLauncher.StartAsync<CompatHostActivity>();
        try
        {
            await Assert.That(activity.Fragment.TitleText).IsNotNull();
            await Assert.That(activity.Fragment.Input).IsNotNull();
            await Assert.That(activity.Fragment.Input!.Id).IsEqualTo(Resource.Id.renamed_input);
        }
        finally
        {
            await ActivityLauncher.FinishAsync(activity);
        }
    }

    /// <summary>Creates an <see cref="AutoWiredHost"/> against a detached parent.</summary>
    /// <param name="strategy">The wire-up strategy.</param>
    /// <returns>The host.</returns>
    private static AutoWiredHost CreateHost(ResolveStrategy strategy) =>
        new(ActivityLauncher.TargetContext, new FrameLayout(ActivityLauncher.TargetContext), strategy);

    /// <summary>Runs <paramref name="action"/> and returns the exception it throws.</summary>
    /// <param name="action">The work that is expected to throw.</param>
    /// <returns>The exception, or <see langword="null"/> when nothing was thrown.</returns>
    private static Exception? Catch(Func<object?> action)
    {
        try
        {
            _ = action();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
