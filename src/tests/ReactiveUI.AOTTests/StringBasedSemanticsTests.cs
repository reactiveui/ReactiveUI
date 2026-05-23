// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Executors;

namespace ReactiveUI.AOT.Tests;

/// <summary>
/// Contains unit tests that verify the behavior of string-based property observation and change notification mechanisms
/// in reactive objects.
/// </summary>
[TestExecutor<AppBuilderTestExecutor>]
public class StringBasedSemanticsTests
{
    /// <summary>
    /// The minimum number of emissions expected from a basic initial-and-update sequence.
    /// </summary>
    private const int MinInitialAndUpdateEmissions = 2;

    /// <summary>
    /// The number of distinct emissions expected from the distinct-until-changed sequence.
    /// </summary>
    private const int ExpectedDistinctEmissions = 3;

    /// <summary>
    /// ObservableForProperty (string) should emit an initial value followed by updates.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task ObservableForProperty_String_Basic_InitialAndUpdate()
    {
        var obj = new TestReactiveObject();
        var seen = new List<string?>();

        obj.ObservableForProperty<TestReactiveObject, string?>(nameof(TestReactiveObject.TestProperty), false, false)
            .Select(static x => x.Value)
            .Subscribe(seen.Add);

        // initial emission is null, then updated value
        obj.TestProperty = "v1";

        using (Assert.Multiple())
        {
            await Assert.That(seen).Count().IsGreaterThanOrEqualTo(MinInitialAndUpdateEmissions);
            await Assert.That(seen[0]).IsNull();
            await Assert.That(seen[^1]).IsEqualTo("v1");
        }
    }

    /// <summary>
    /// ObservableForProperty (string) with beforeChange should provide the previous value when the property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task ObservableForProperty_String_BeforeChange_FiresOldValue()
    {
        var obj = new TestReactiveObject { TestProperty = "start" };
        string? observed = null;

        obj.ObservableForProperty<TestReactiveObject, string?>(nameof(TestReactiveObject.TestProperty), true, true)
            .Select(x => x.Value)
            .Subscribe(v => observed = v);

        obj.TestProperty = "next";

        await Assert.That(observed).IsEqualTo("start");
    }

    /// <summary>
    /// WhenAnyValue (string) should apply DistinctUntilChanged by default and include an initial emission.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task WhenAnyValue_String_IsDistinct()
    {
        var obj = new TestReactiveObject();
        var seen = new List<string?>();

        obj.WhenAnyValue<TestReactiveObject, string?>(nameof(TestReactiveObject.TestProperty))
            .Subscribe(seen.Add);

        obj.TestProperty = "same";
        obj.TestProperty = "same"; // should be filtered by distinct
        obj.TestProperty = "other";

        using (Assert.Multiple())
        {
            // initial null + "same" + "other" => 3 distinct emissions
            await Assert.That(seen).Count().IsGreaterThanOrEqualTo(ExpectedDistinctEmissions);
            await Assert.That(seen.TakeLast(ExpectedDistinctEmissions).ToArray()).IsEquivalentTo([null, "same", "other"]);
        }
    }

    /// <summary>
    /// WhenAnyValue (string) tuple overload should combine the latest values from two properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the string/expression-based reflection API to verify runtime behavior.")]
    public async Task WhenAnyValue_String_TupleCombine_Works()
    {
        var obj = new TestReactiveObject();
        var tuples = new List<(string?, string?)>();

        obj.WhenAnyValue<TestReactiveObject, string?, string?>(
            nameof(TestReactiveObject.TestProperty),
            nameof(TestReactiveObject.ComputedProperty)).Subscribe(tuples.Add);

        obj.TestProperty = "value";

        await Assert.That(tuples).IsNotEmpty();
        var last = tuples[^1];
        await Assert.That(last.Item1).IsEqualTo("value");
    }
}
