// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI;

/// <summary>
/// Attribute that marks a resource for wiring.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class WireUpResourceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireUpResourceAttribute"/> class.
    /// </summary>
    public WireUpResourceAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireUpResourceAttribute"/> class.
    /// </summary>
    /// <param name="resourceName">Name of the resource.</param>
    public WireUpResourceAttribute(string? resourceName) => ResourceNameOverride = resourceName;

    /// <summary>
    /// Gets the resource name override.
    /// </summary>
    public string? ResourceNameOverride { get; }
}
