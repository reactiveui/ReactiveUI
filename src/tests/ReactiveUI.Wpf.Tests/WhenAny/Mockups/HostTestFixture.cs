// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny.Mockups;

/// <summary>
/// A host test fixture.
/// </summary>
public sealed class HostTestFixture : ReactiveObject
{
    /// <summary>
    /// Backing field for the <see cref="OwnerName"/> property.
    /// </summary>
    private readonly ObservableAsPropertyHelper<string?> _ownerName;

    /// <summary>
    /// Backing field for the <see cref="Child"/> property.
    /// </summary>
    private TestFixture? _Child;

    /// <summary>
    /// Backing field for the <see cref="Owner"/> property.
    /// </summary>
    private OwnerClass? _owner;

    /// <summary>
    /// Backing field for the <see cref="PocoChild"/> property.
    /// </summary>
    private NonObservableTestFixture? _PocoChild;

    /// <summary>
    /// Backing field for the <see cref="SomeOtherParam"/> property.
    /// </summary>
    private int _SomeOtherParam;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostTestFixture"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:\"this\" should not be exposed from constructors",
        Justification = "OAPH/WhenAny initialization requires 'this'; single-threaded test fixture.")]
    public HostTestFixture() =>
        _ownerName = this.WhenAnyValue(static x => x.Owner)
            .WhereNotNull()
            .Select(static owner => owner.WhenAnyValue(static x => x.Name))
            .Switch()
            .ToProperty(this, static x => x.OwnerName);

    /// <summary>
    /// Gets the name of the owner.
    /// </summary>
    public string? OwnerName => _ownerName.Value;

    /// <summary>
    /// Gets or sets the child.
    /// </summary>
    public TestFixture? Child
    {
        get => _Child;
        set => this.RaiseAndSetIfChanged(ref _Child, value);
    }

    /// <summary>
    /// Gets or sets the owner.
    /// </summary>
    public OwnerClass? Owner
    {
        get => _owner;
        set => this.RaiseAndSetIfChanged(ref _owner, value);
    }

    /// <summary>
    /// Gets or sets the poco child.
    /// </summary>
    public NonObservableTestFixture? PocoChild
    {
        get => _PocoChild;
        set => this.RaiseAndSetIfChanged(ref _PocoChild, value);
    }

    /// <summary>
    /// Gets or sets some other parameter.
    /// </summary>
    public int SomeOtherParam
    {
        get => _SomeOtherParam;
        set => this.RaiseAndSetIfChanged(ref _SomeOtherParam, value);
    }
}
