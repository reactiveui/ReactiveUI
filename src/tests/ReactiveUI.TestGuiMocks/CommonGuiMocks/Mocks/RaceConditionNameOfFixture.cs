// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

/// <summary>A fixture for RaceCondition and NameOf.</summary>
public class RaceConditionNameOfFixture : ReactiveObject
{
    /// <summary>Backing helper for the <see cref="A"/> property.</summary>
    private readonly ObservableAsPropertyHelper<bool> _a;

    /// <summary>Initializes a new instance of the <see cref="RaceConditionNameOfFixture"/> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "canonical ObservableAsPropertyHelper initialization requires 'this' in the constructor; the single-threaded fixture never exposes the half-built instance.")]
    public RaceConditionNameOfFixture() =>

        // We need to generate a value on subscription
        // which is different than the default value.
        // This triggers the property change firing
        // upon subscription in the ObservableAsPropertyHelper
        // constructor.
        Signal.Emit(true)
            .Do(_ => Count++)
            .ToProperty(this, nameof(A), out _a);

    /// <summary>Gets or sets the count.</summary>
    public int Count { get; set; }

    /// <summary>Gets a value indicating whether this <see cref="RaceConditionNameOfFixture"/> is a.</summary>
    public bool A => _a.Value;
}
