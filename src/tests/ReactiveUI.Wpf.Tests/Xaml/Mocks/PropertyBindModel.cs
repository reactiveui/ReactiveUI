// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>A property bind model.</summary>
/// <remarks>
/// It raises change notifications so a path through it is observed to its end: the binding engine reports a link
/// that raises none (RXUIBIND010), because the observation would stop following the path there.
/// </remarks>
public class PropertyBindModel : ReactiveObject
{
    /// <summary>Gets or sets another thing.</summary>
    public string? AnotherThing
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets a thing.</summary>
    public int AThing
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
