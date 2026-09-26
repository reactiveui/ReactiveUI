// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Computes <see cref="CanEnroll"/> with an <see cref="ObservableAsPropertyHelper{T}"/> instead of setting it from a
/// subscription. The property has no setter, so nothing else in the codebase can write the wrong value into it.
/// </summary>
[System.Diagnostics.DebuggerDisplay("CanEnroll = {CanEnroll}")]
public sealed class PreferEnrollmentViewModel : ReactiveObject, IDisposable
{
    /// <summary>Backs <see cref="CanEnroll"/>.</summary>
    private readonly ObservableAsPropertyHelper<bool> _canEnroll;

    /// <summary>Initializes a new instance of the <see cref="PreferEnrollmentViewModel"/> class.</summary>
    public PreferEnrollmentViewModel() =>
        _canEnroll = this.WhenAny(x => x.RosterFetched, x => x.RegistrarFree, static (fetched, free) => fetched.Value && free.Value)
            .ToProperty(this, nameof(CanEnroll));

    /// <summary>Gets or sets a value indicating whether the course roster has been fetched.</summary>
    public bool RosterFetched
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets a value indicating whether the registrar is free to process a new enrollment.</summary>
    public bool RegistrarFree
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets a value indicating whether a student can enroll. Read-only: it can only change through the pipeline above.</summary>
    public bool CanEnroll => _canEnroll.Value;

    /// <inheritdoc/>
    public void Dispose() => _canEnroll.Dispose();
}
