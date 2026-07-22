// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>A mock view model.</summary>
public class FakeCollectionViewModel : ReactiveObject
{
    /// <summary>Backing helper that projects the model number as a string.</summary>
    private readonly ObservableAsPropertyHelper<string?> _numberAsString;

    /// <summary>Initializes a new instance of the <see cref="FakeCollectionViewModel" /> class.</summary>
    /// <param name="model">The model.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "SST2403:'this' escapes 'FakeCollectionViewModel' before construction finishes",
        Justification = "canonical ObservableAsPropertyHelper initialization requires 'this' in the constructor; " +
            "the single-threaded fixture never exposes the half-built instance.")]
    public FakeCollectionViewModel(FakeCollectionModel model)
    {
        Model = model;

        _ = this.WhenAny(static x => x.Model.SomeNumber, static x => x.Value.ToString()).ToProperty(
            this,
            static x => x.NumberAsString,
            out _numberAsString);
    }

    /// <summary>Gets the number as string.</summary>
    public string? NumberAsString => _numberAsString.Value;

    /// <summary>Gets the model.</summary>
    public FakeCollectionModel Model { get; protected set; }
}
