// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AOTTests;

/// <summary>
/// Tests for AOT-friendly mapping and ResolveView contract usage.
/// </summary>
[TestFixture]
public class ViewLocatorAOTMappingTests
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocatorAOTMappingTests"/> class.
    /// </summary>
    public ViewLocatorAOTMappingTests()
    {
        RxApp.EnsureInitialized();
    }

    /// <summary>
    /// Map/Resolve with contract and default fallback works.
    /// </summary>
    [Test]
    public void Map_ResolveView_UsesAOTMappingWithContract()
    {
        var locator = new DefaultViewLocator();

        // Register contract-specific and default mappings
        locator.Map<VmA, ViewA>(static () => new ViewA(), contract: "mobile")
            .Map<VmA, ViewADefault>(static () => new ViewADefault()); // default

        var vm = new VmA();

        var viewMobile = locator.ResolveView(vm, "mobile");
        Assert.That(viewMobile, Is.TypeOf<ViewA>());

        var viewDefaultFromExplicit = locator.ResolveView(vm, string.Empty);
        Assert.That(viewDefaultFromExplicit, Is.TypeOf<ViewADefault>());

        // Unknown contract falls back to default mapping
        var viewFallback = locator.ResolveView(vm, "unknown");
        Assert.That(viewFallback, Is.TypeOf<ViewADefault>());
    }

    /// <summary>
    /// Unmap removes a mapping for a contract.
    /// </summary>
    [Test]
    public void Unmap_RemovesMapping()
    {
        var locator = new DefaultViewLocator();
        locator.Map<VmB, ViewB>(static () => new ViewB(), contract: "c1");

        var vm = new VmB();
        Assert.That(locator.ResolveView(vm, "c1"), Is.TypeOf<ViewB>());

        locator.Unmap<VmB>("c1");
        Assert.That(locator.ResolveView(vm, "c1"), Is.Null);
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

    private sealed class VmA : ReactiveObject
    {
    }

    private sealed class VmB : ReactiveObject
    {
    }

    private sealed class ViewA : IViewFor<VmA>
    {
        object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (VmA?)value; }

        public VmA? ViewModel { get; set; }
    }
}
