// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;

using TUnit.Core.Executors;

namespace ReactiveUI.AOT.Tests;

/// <summary>
/// Tests for ViewLocator AOT mappings.
/// </summary>
[TestExecutor<AppBuilderTestExecutor>]
public class ViewLocatorAOTMappingTests
{
    /// <summary>
    /// Map/Resolve with contract and default fallback works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Map_ResolveView_UsesAOTMappingWithContract()
    {
        var locator = new DefaultViewLocator();

        // Register contract-specific and default mappings
        locator.Map<VmA, ViewA>(static () => new ViewA(), contract: "mobile")
            .Map<VmA, ViewADefault>(static () => new ViewADefault()); // default

        var viewMobile = locator.ResolveView<VmA>("mobile");
        await Assert.That(viewMobile).IsTypeOf<ViewA>();

        var viewDefaultFromExplicit = locator.ResolveView<VmA>(string.Empty);
        await Assert.That(viewDefaultFromExplicit).IsTypeOf<ViewADefault>();

        // Unknown contract returns null (no fallback in ViewLocator)
        var viewUnknown = locator.ResolveView<VmA>("unknown");
        await Assert.That(viewUnknown).IsNull();
    }

    /// <summary>
    /// Unmap removes a mapping for a contract.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Unmap_RemovesMapping()
    {
        var locator = new DefaultViewLocator();
        locator.Map<VmB, ViewB>(static () => new ViewB(), contract: "c1");

        await Assert.That(locator.ResolveView<VmB>("c1")).IsTypeOf<ViewB>();

        locator.Unmap<VmB>("c1");
        await Assert.That(locator.ResolveView<VmB>("c1")).IsNull();
    }

    /// <summary>
    /// Tests that AOT mapping is used without contract.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Map_ResolveView_UsesAOTMappingWithoutContract()
    {
        var locator = new DefaultViewLocator();
        locator.Map<VmA, ViewA>(static () => new ViewA());

        var view = locator.ResolveView<VmA>();

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<ViewA>();
    }

    private sealed class ViewADefault : IViewFor<VmA>
    {
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (VmA?)value; }

        public VmA? ViewModel { get; set; }
    }

    private sealed class ViewB : IViewFor<VmB>
    {
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (VmB?)value; }

        public VmB? ViewModel { get; set; }
    }

    private sealed class VmA : ReactiveObject;

    private sealed class VmB : ReactiveObject;

    private sealed class ViewA : IViewFor<VmA>
    {
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (VmA?)value; }

        public VmA? ViewModel { get; set; }
    }
}
