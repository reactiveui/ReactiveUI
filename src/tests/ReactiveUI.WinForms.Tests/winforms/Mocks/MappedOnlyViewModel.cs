// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A routable view model no class implements <see cref="IViewFor{T}"/> for, so only a <c>Map</c> registration gives it a view.</summary>
public class MappedOnlyViewModel : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string? UrlPathSegment => "mapped-only";

    /// <inheritdoc/>
    public IScreen HostScreen => null!;
}
