// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A view a test registers only with the service locator, as <see cref="IViewFor{T}"/> of
/// <see cref="SplatOnlyViewModel"/>. It is excluded from view registration, so the source generator writes no lookup
/// entry for it.
/// </summary>
[ExcludeFromViewRegistration]
public class SplatOnlyView : Control, IViewFor<SplatOnlyViewModel>
{
    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SplatOnlyViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SplatOnlyViewModel?)value;
    }
}
