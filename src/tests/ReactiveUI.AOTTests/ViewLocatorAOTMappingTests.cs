// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Executors;

namespace ReactiveUI.AOT.Tests;

/// <summary>Tests for ViewLocator AOT mappings.</summary>
[TestExecutor<AppBuilderTestExecutor>]
public class ViewLocatorAOTMappingTests
{
    /// <summary>Map/Resolve with contract and default fallback works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Map_ResolveView_UsesAOTMappingWithContract()
    {
        var locator = new DefaultViewLocator();

        // Register contract-specific and default mappings
        locator.Map<VmA, ViewA>(static () => new(), "mobile")
            .Map<VmA, ViewADefault>(static () => new()); // default

        var viewMobile = locator.ResolveView<VmA>("mobile");
        await Assert.That(viewMobile).IsTypeOf<ViewA>();

        var viewDefaultFromExplicit = locator.ResolveView<VmA>(string.Empty);
        await Assert.That(viewDefaultFromExplicit).IsTypeOf<ViewADefault>();

        // Unknown contract returns null (no fallback in ViewLocator)
        var viewUnknown = locator.ResolveView<VmA>("unknown");
        await Assert.That(viewUnknown).IsNull();
    }

    /// <summary>Unmap removes a mapping for a contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Unmap_RemovesMapping()
    {
        var locator = new DefaultViewLocator();
        locator.Map<VmB, ViewB>(static () => new(), "c1");

        await Assert.That(locator.ResolveView<VmB>("c1")).IsTypeOf<ViewB>();

        locator.Unmap<VmB>("c1");
        await Assert.That(locator.ResolveView<VmB>("c1")).IsNull();
    }

    /// <summary>Tests that AOT mapping is used without contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Map_ResolveView_UsesAOTMappingWithoutContract()
    {
        var locator = new DefaultViewLocator();
        locator.Map<VmA, ViewA>(static () => new());

        var view = locator.ResolveView<VmA>();

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<ViewA>();
    }

    /// <summary>Default view for <see cref="VmA"/> used to test fallback resolution.</summary>
    private sealed class ViewADefault : IViewFor<VmA>
    {
        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (VmA?)value;
        }

        /// <inheritdoc/>
        public VmA? ViewModel { get; set; }
    }

    /// <summary>View for <see cref="VmB"/> used to test contract-based resolution.</summary>
    private sealed class ViewB : IViewFor<VmB>
    {
        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (VmB?)value;
        }

        /// <inheritdoc/>
        public VmB? ViewModel { get; set; }
    }

    /// <summary>Sample view model used for view locator resolution tests.</summary>
    [SuppressMessage("Minor Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class VmA : ReactiveObject;

    /// <summary>Sample view model used for view locator resolution tests.</summary>
    [SuppressMessage("Minor Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class VmB : ReactiveObject;

    /// <summary>View for <see cref="VmA"/> used to test contract-based resolution.</summary>
    private sealed class ViewA : IViewFor<VmA>
    {
        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (VmA?)value;
        }

        /// <inheritdoc/>
        public VmA? ViewModel { get; set; }
    }
}
