// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;
using WinUIActivationForViewFetcher = ReactiveUI.WinUI.ActivationForViewFetcher;
using WinUIRegistrations = ReactiveUI.WinUI.Registrations;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for the WinUI <see cref="WinUIRegistrations"/> platform module.</summary>
/// <remarks>
/// The WinUI types are reached through aliases: this namespace sits under <c>ReactiveUI</c>, so an unqualified
/// name would bind to the shared implementation in <c>ReactiveUI</c> before any using directive is consulted.
/// </remarks>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class RegistrationsTests
{
    /// <summary>The number of binding type converters the WinUI platform contributes.</summary>
    private const int WinUIConverterCount = 2;

    /// <summary>Verifies a null registrar is rejected rather than producing a partial registration.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_WithNoRegistrar_Throws()
    {
        var registrations = new WinUIRegistrations();

        await Assert.That(() => registrations.Register(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies the platform services a WinUI application needs are all contributed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_ContributesTheWinUIPlatformServices()
    {
        var registrar = new RecordingRegistrar();

        new WinUIRegistrations().Register(registrar);

        using (Assert.Multiple())
        {
            _ = await Assert.That(registrar.For(typeof(IActivationForViewFetcher))).HasSingleItem();
            _ = await Assert.That(registrar.For(typeof(IPlatformOperations))).HasSingleItem();
            _ = await Assert.That(registrar.For(typeof(ICreatesObservableForProperty))).HasSingleItem();
            _ = await Assert.That(registrar.For(typeof(IPropertyBindingHook))).HasSingleItem();
            _ = await Assert.That(registrar.For(typeof(IBindingFallbackConverter))).HasSingleItem();
            await Assert.That(registrar.For(typeof(IBindingTypeConverter))).Count().IsEqualTo(WinUIConverterCount);
        }
    }

    /// <summary>Verifies the registered services are the WinUI implementations rather than the shared ones.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_ContributesTheWinUIImplementations()
    {
        var registrar = new RecordingRegistrar();

        new WinUIRegistrations().Register(registrar);

        using (Assert.Multiple())
        {
            _ = await Assert.That(registrar.For(typeof(IActivationForViewFetcher))[0]).IsTypeOf<WinUIActivationForViewFetcher>();
            _ = await Assert.That(registrar.For(typeof(IPlatformOperations))[0]).IsTypeOf<PlatformOperations>();
            _ = await Assert.That(registrar.For(typeof(ICreatesObservableForProperty))[0])
                .IsTypeOf<DependencyObjectObservableForProperty>();
            _ = await Assert.That(registrar.For(typeof(IPropertyBindingHook))[0]).IsTypeOf<AutoDataTemplateBindingHook>();
        }
    }

    /// <summary>Verifies registering suppresses the command-binding advice that does not apply to WinUI.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_SuppressesTheViewCommandBindingMessage()
    {
        RxSchedulers.SuppressViewCommandBindingMessage = false;

        new WinUIRegistrations().Register(new RecordingRegistrar());

        await Assert.That(RxSchedulers.SuppressViewCommandBindingMessage).IsTrue();
    }
}
