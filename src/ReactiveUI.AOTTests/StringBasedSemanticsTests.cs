// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Verifies the string-based ObservableForProperty and WhenAnyValue semantics.
/// Ensures initial emission, beforeChange behavior, distinct filtering, and tuple combinations.
/// </summary>
[TestFixture]
public class StringBasedSemanticsTests
{
    /// <summary>
    /// ObservableForProperty (string) should emit an initial value followed by updates.
    /// </summary>
    [Test]
    public void ObservableForProperty_String_Basic_InitialAndUpdate()
    {
        var obj = new TestReactiveObject();
        var seen = new List<string?>();

        obj.ObservableForProperty<TestReactiveObject, string?>(nameof(TestReactiveObject.TestProperty), beforeChange: false, skipInitial: false)
           .Select(x => x.Value)
           .Subscribe(seen.Add);

        // initial emission is null, then updated value
        obj.TestProperty = "v1";

        Assert.That(seen.Count >= 2, Is.True);
        Assert.That(seen[0], Is.Null);
        Assert.That(seen[^1], Is.EqualTo("v1"));
    }

    /// <summary>
    /// ObservableForProperty (string) with beforeChange should provide the previous value when the property changes.
    /// </summary>
    [Test]
    public void ObservableForProperty_String_BeforeChange_FiresOldValue()
    {
        var obj = new TestReactiveObject { TestProperty = "start" };
        string? observed = null;

        obj.ObservableForProperty<TestReactiveObject, string?>(nameof(TestReactiveObject.TestProperty), beforeChange: true, skipInitial: true)
           .Select(x => x.Value)
           .Subscribe(v => observed = v);

        obj.TestProperty = "next";

        Assert.That(observed, Is.EqualTo("start"));
    }

    /// <summary>
    /// WhenAnyValue (string) should apply DistinctUntilChanged by default and include an initial emission.
    /// </summary>
    [Test]
    public void WhenAnyValue_String_IsDistinct()
    {
        var obj = new TestReactiveObject();
        var seen = new List<string?>();

        obj.WhenAnyValue<TestReactiveObject, string?>(nameof(TestReactiveObject.TestProperty))
           .Subscribe(seen.Add);

        obj.TestProperty = "same";
        obj.TestProperty = "same"; // should be filtered by distinct
        obj.TestProperty = "other";

        // initial null + "same" + "other" => 3 distinct emissions
        Assert.That(seen.Count >= 3, Is.True);
        Assert.That(seen.TakeLast(3).ToArray(), Is.EqualTo(new[] { null, "same", "other" }));
    }

    /// <summary>
    /// WhenAnyValue (string) tuple overload should combine the latest values from two properties.
    /// </summary>
    [Test]
    public void WhenAnyValue_String_TupleCombine_Works()
    {
        var obj = new TestReactiveObject();
        var tuples = new List<(string?, string?)>();

        obj.WhenAnyValue<TestReactiveObject, string?, string?>(nameof(TestReactiveObject.TestProperty), nameof(TestReactiveObject.ComputedProperty))
           .Subscribe(tuples.Add);

        obj.TestProperty = "value";

        Assert.That(tuples.Count >= 1, Is.True);
        var last = tuples[^1];
        Assert.That(last.Item1, Is.EqualTo("value"));
    }
}
