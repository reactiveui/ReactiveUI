// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Tests for AOT-friendly mapping and ResolveView contract usage.
/// </summary>
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
    [Fact]
    public void Map_ResolveView_UsesAOTMappingWithContract()
    {
        var locator = new DefaultViewLocator();

        // Register contract-specific and default mappings
        locator.Map<VmA, ViewA>(() => new ViewA(), contract: "mobile")
            .Map<VmA, ViewADefault>(() => new ViewADefault()); // default

        var vm = new VmA();

        var viewMobile = locator.ResolveView(vm, "mobile");
        Assert.IsType<ViewA>(viewMobile);

        var viewDefaultFromExplicit = locator.ResolveView(vm, string.Empty);
        Assert.IsType<ViewADefault>(viewDefaultFromExplicit);

        // Unknown contract falls back to default mapping
        var viewFallback = locator.ResolveView(vm, "unknown");
        Assert.IsType<ViewADefault>(viewFallback);
    }

    /// <summary>
    /// Unmap removes a mapping for a contract.
    /// </summary>
    [Fact]
    public void Unmap_RemovesMapping()
    {
        var locator = new DefaultViewLocator();
        locator.Map<VmB, ViewB>(() => new ViewB(), contract: "c1");

        var vm = new VmB();
        Assert.IsType<ViewB>(locator.ResolveView(vm, "c1"));

        locator.Unmap<VmB>("c1");
        Assert.Null(locator.ResolveView(vm, "c1"));
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
