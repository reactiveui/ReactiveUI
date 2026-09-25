// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>The contracts that pick between several views of the same view model.</summary>
internal static class ViewContracts
{
    /// <summary>The one-line view for narrow windows and widgets.</summary>
    public const string Compact = "compact";

    /// <summary>A printable view, which the app does not have.</summary>
    public const string Print = "print";
}
