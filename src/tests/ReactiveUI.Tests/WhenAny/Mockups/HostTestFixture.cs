// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny.Mockups;

/// <summary>A host test fixture.</summary>
public class HostTestFixture : ReactiveObject
{
    /// <summary>Backs the <see cref="OwnerName" /> property with the owner's name observed via WhenAnyValue.</summary>
    private readonly ObservableAsPropertyHelper<string?> _ownerName;

    /// <summary>Initializes a new instance of the <see cref="HostTestFixture" /> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:Make sure the use of this in constructors is safe here",
        Justification = "OAPH initialization requires 'this' in the constructor; single-threaded test fixture.")]
    public HostTestFixture() =>
        _ownerName = ObservableMixins.WhereNotNull(this.WhenAnyValue(static x => x.Owner))
            .Select(static owner => owner.WhenAnyValue(static x => x.Name))
            .Switch()
            .ToProperty(this, static x => x.OwnerName);

    /// <summary>Gets the name of the owner.</summary>
    public string? OwnerName => _ownerName.Value;

    /// <summary>Gets or sets the child.</summary>
    public TestFixture? Child
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the owner.</summary>
    public OwnerClass? Owner
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the poco child.</summary>
    public NonObservableTestFixture? PocoChild
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets some other parameter.</summary>
    public int SomeOtherParam
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
