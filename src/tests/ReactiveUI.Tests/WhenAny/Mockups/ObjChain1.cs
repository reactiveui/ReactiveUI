// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny.Mockups;

/// <summary>The first link in a chain of nested reactive objects used for deep property observation tests.</summary>
public class ObjChain1 : ReactiveObject
{
    /// <summary>Gets or sets the nested model.</summary>
    public ObjChain2 Model
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();
}
