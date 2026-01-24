// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Specifies that a resource should be wired up to the target element, optionally using a specified resource name
/// override.
/// </summary>
/// <remarks>Apply this attribute to a class, method, property, or other code element to indicate that a resource
/// should be associated with it. Use the optional resource name override to specify a custom resource name if the
/// default naming convention does not apply.</remarks>
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
