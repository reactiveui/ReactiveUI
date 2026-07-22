// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.AOT.Tests;

/// <summary>Test ReactiveObject for AOT compatibility testing.</summary>
public class TestReactiveObject : ReactiveObject
{
    /// <summary>The backing helper that produces the computed property value.</summary>
    private readonly ObservableAsPropertyHelper<string> _computedProperty;

    /// <summary>Initializes a new instance of the <see cref="TestReactiveObject"/> class.</summary>
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with RequiresUnreferencedCodeAttribute may break when trimming",
        Justification = "Test deliberately exercises the expression-based reflection API to verify runtime behavior.")]
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "canonical ObservableAsPropertyHelper initialization requires 'this' in the constructor; the single-threaded test object never exposes the half-built instance.")]
    public TestReactiveObject() =>
        _computedProperty = this.WhenAnyValue(static x => x.TestProperty)
            .Select(static x => $"Computed: {x}")
            .ToProperty(this, nameof(ComputedProperty));

    /// <summary>Gets or sets the test property.</summary>
    public string? TestProperty
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the computed property value.</summary>
    public string ComputedProperty => _computedProperty.Value;
}
