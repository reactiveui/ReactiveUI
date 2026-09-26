// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Sets <see cref="CanEnroll"/> from a subscription instead of computing it. Because the property has a public setter,
/// nothing stops other code in the class, or anywhere else, from writing to it directly.
/// </summary>
[System.Diagnostics.DebuggerDisplay("CanEnroll = {CanEnroll}")]
public sealed class AvoidEnrollmentViewModel : ReactiveObject, IDisposable
{
    /// <summary>The subscription that sets <see cref="CanEnroll"/>; disposing it does not stop other code from writing to the property directly.</summary>
    private readonly IDisposable _subscription;

    /// <summary>Initializes a new instance of the <see cref="AvoidEnrollmentViewModel"/> class.</summary>
    public AvoidEnrollmentViewModel() =>
        _subscription = this.WhenAny(x => x.RosterFetched, x => x.RegistrarFree, static (fetched, free) => fetched.Value && free.Value)
            .Subscribe(canEnroll => CanEnroll = canEnroll);

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

    /// <summary>Gets or sets a value indicating whether a student can enroll. A plain settable property: anyone can overwrite it.</summary>
    public bool CanEnroll
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public void Dispose() => _subscription.Dispose();
}
