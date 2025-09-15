// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Test ReactiveObject for AOT compatibility testing.
/// </summary>
public class TestReactiveObject : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<string> _computedProperty;
    private string? _testProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestReactiveObject"/> class.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "AOT compatibility tests deliberately use AOT-incompatible methods to test suppression scenarios")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "AOT compatibility tests deliberately use AOT-incompatible methods to test suppression scenarios")]
    public TestReactiveObject()
    {
        _computedProperty = this.WhenAnyValue(static x => x.TestProperty)
            .Select(static x => $"Computed: {x}")
            .ToProperty(this, nameof(ComputedProperty));
    }

    /// <summary>
    /// Gets or sets the test property.
    /// </summary>
    public string? TestProperty
    {
        get => _testProperty;
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "AOT compatibility tests deliberately use AOT-incompatible methods to test suppression scenarios")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "AOT compatibility tests deliberately use AOT-incompatible methods to test suppression scenarios")]
        set => this.RaiseAndSetIfChanged(ref _testProperty, value);
    }

    /// <summary>
    /// Gets the computed property value.
    /// </summary>
    public string ComputedProperty => _computedProperty.Value;
}
