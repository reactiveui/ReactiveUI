// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests what the AndroidX builder module registers when an app starts ReactiveUI with <c>WithAndroidX</c>.</summary>
/// <remarks><see cref="AndroidAssemblyHooks.StartReactiveUI"/> builds the app once, before these tests run.</remarks>
public class AndroidRegistrationTests
{
    /// <summary>The main-thread scheduler is the Android main looper.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task MainThreadScheduler_IsTheMainHandlerSequencer()
    {
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(HandlerSequencer.Main);
        await Assert.That(AndroidXReactiveUIBuilderExtensions.AndroidXMainThreadScheduler).IsSameReferenceAs(HandlerSequencer.Main);
    }

    /// <summary>The main-thread scheduler really runs work on the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task MainThreadScheduler_RunsWorkOnTheMainThread()
    {
        var onMain = Signal.Emit(0, RxSchedulers.MainThreadScheduler).Select(static _ => MainThread.IsCurrent).FirstValueAsync(out var subscription);
        using (subscription)
        {
            await Assert.That(await onMain.WithTimeout("the main-thread value")).IsTrue();
        }
    }

    /// <summary>The platform operations service is the Android implementation.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task PlatformOperations_IsRegistered()
    {
        var operations = Locator.Current.GetService<IPlatformOperations>();

        await Assert.That(operations).IsTypeOf<PlatformOperations>();
        await Assert.That(operations!.GetOrientation()).IsNotNull();
    }

    /// <summary>The suspension driver is the bundle driver.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SuspensionDriver_IsTheBundleDriver() =>
        await Assert.That(Locator.Current.GetService<ISuspensionDriver>()).IsTypeOf<BundleSuspensionDriver>();

    /// <summary>A reactive activity can be activated because the activation fetcher is registered.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ActivationFetcher_IsRegistered() =>
        await Assert.That(Locator.Current.GetServices<IActivationForViewFetcher>()).IsNotEmpty();
}
