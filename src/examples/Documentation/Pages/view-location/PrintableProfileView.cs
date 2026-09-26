// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// A profile screen laid out for printing, picked by the <see cref="ViewContracts.Print"/> contract and also
/// resolved through the service locator. Marked <see cref="ExcludeFromViewRegistrationAttribute"/> so the locator
/// finds it only through that registration.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("PrintableProfileView ViewModel = {ViewModel}")]
public sealed class PrintableProfileView : ReactiveObject, IViewFor<SchoolProfileViewModel>
{
    /// <summary>Gets or sets the profile this view shows.</summary>
    public SchoolProfileViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SchoolProfileViewModel?)value;
    }
}
