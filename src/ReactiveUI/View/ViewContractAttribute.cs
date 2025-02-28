// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Allows an additional string to make view resolution more specific than
/// just a type. When applied to your <see cref="IViewFor{T}"/> -derived
/// View, you can select between different Views for a single ViewModel
/// instance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ViewContractAttribute"/> class.
/// Constructs the ViewContractAttribute with a specific contract value.
/// </remarks>
/// <param name="contract">The value of the contract for view
/// resolution.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewContractAttribute(string contract) : Attribute
{
    /// <summary>
    /// Gets the contract to use when resolving the view in the Splat Dependency Injection engine.
    /// </summary>
    public string Contract { get; } = contract;
}
