// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

/// <summary>A fixture for demonstrating race conditions.</summary>
/// <seealso cref="ReactiveObject" />
public class RaceConditionFixture : ReactiveObject
{
    /// <summary>Backing helper for the <see cref="A"/> property.</summary>
    private readonly ObservableAsPropertyHelper<bool> _a;

    /// <summary>Initializes a new instance of the <see cref="RaceConditionFixture"/> class.</summary>
    /// <remarks>
    /// Emits a value on subscription that differs from the default, which triggers the property
    /// change firing during the <see cref="ObservableAsPropertyHelper{T}"/> constructor - the race
    /// this fixture exists to reproduce.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "canonical ObservableAsPropertyHelper initialization requires 'this' in the constructor; the single-threaded fixture never exposes the half-built instance.")]
    public RaceConditionFixture() => Signal.Emit(true).Do(_ => Count++).ToProperty(this, x => x.A, out _a);

    /// <summary>Gets or sets the count.</summary>
    public int Count { get; set; }

    /// <summary>Gets a value indicating whether this <see cref="RaceConditionFixture"/> is a.</summary>
    public bool A => _a.Value;
}
