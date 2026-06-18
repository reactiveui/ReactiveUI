// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>A collection model.</summary>
public class FakeCollectionModel : ReactiveObject
{
    /// <summary>Gets or sets a value indicating whether this instance is hidden.</summary>
    public bool IsHidden
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets some number.</summary>
    public int SomeNumber
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
