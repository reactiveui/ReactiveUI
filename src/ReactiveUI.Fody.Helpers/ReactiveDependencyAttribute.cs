// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI.Fody.Helpers;

/// <summary>
/// Attribute that marks a property as a Reactive Dependency.
/// </summary>
/// <seealso cref="System.Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="ReactiveDependencyAttribute"/> class.
/// </remarks>
/// <param name="targetName">Name of the target.</param>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ReactiveDependencyAttribute(string targetName) : Attribute
{
    /// <summary>
    /// Gets the name of the backing property.
    /// </summary>
    public string Target { get; } = targetName;

    /// <summary>
    /// Gets or sets the target property on the backing property.
    /// </summary>
    public string? TargetProperty { get; set; }
}
