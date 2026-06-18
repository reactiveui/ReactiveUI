// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>A reactive project model used for serialization tests.</summary>
/// <seealso cref="ReactiveObject" />
[DataContract]
public class Project : ReactiveObject
{
    /// <summary>Gets or sets the name.</summary>
    [DataMember]
    [JsonRequired]
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
