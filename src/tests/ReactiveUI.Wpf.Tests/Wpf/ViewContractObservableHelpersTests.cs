// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;
using TUnit.Core.Executors;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="ViewContractObservableHelpers"/>.</summary>
public class ViewContractObservableHelpersTests
{
    /// <summary>The initial orientation returned by the platform.</summary>
    private const string Portrait = "portrait";

    /// <summary>The changed orientation emitted by the host.</summary>
    private const string Landscape = "landscape";

    /// <summary>The runtime size-change signals, including a consecutive duplicate.</summary>
    private static readonly string?[] RuntimeSignals = [Portrait, Landscape, Landscape];

    /// <summary>The distinct runtime contracts expected from the helper.</summary>
    private static readonly string?[] ExpectedRuntimeContracts = [Portrait, Landscape];

    /// <summary>Verifies that test mode suppresses platform view-contract signals.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Create_TestMode_ReturnsSilentObservable()
    {
        var values = ViewContractObservableHelpers.Create(static () => Portrait, Signal.Emit<string?>(Landscape)).Collect();

        await Assert.That(values).IsEmpty();
    }

    /// <summary>Verifies that runtime mode starts with the current orientation and removes consecutive duplicates.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<RuntimeModeTestExecutor>]
    public async Task Create_RuntimeMode_StartsWithOrientationAndRemovesDuplicates()
    {
        var values = ViewContractObservableHelpers.Create(
                static () => Portrait,
                Signal.FromEnumerable(RuntimeSignals))
            .Collect();

        await Assert.That(values).IsEquivalentTo(ExpectedRuntimeContracts);
    }

    /// <summary>Runs a test with runtime mode enabled and restores test mode afterward.</summary>
    public sealed class RuntimeModeTestExecutor : ITestExecutor
    {
        /// <inheritdoc/>
        public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
        {
            ModeDetector.OverrideModeDetector(new FixedModeDetector(false));
            try
            {
                await action();
            }
            finally
            {
                ModeDetector.OverrideModeDetector(new FixedModeDetector(true));
            }
        }
    }

    /// <summary>Returns a fixed unit-test mode value.</summary>
    /// <param name="isTestMode">The mode value to return.</param>
    private sealed class FixedModeDetector(bool isTestMode) : IModeDetector
    {
        /// <inheritdoc/>
        public bool? InUnitTestRunner() => isTestMode;
    }
}
