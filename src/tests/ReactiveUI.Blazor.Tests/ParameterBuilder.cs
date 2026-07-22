// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace ReactiveUI.Blazor.Tests;

/// <summary>Collects strongly typed component parameters for a render, mirroring a fluent parameter builder.</summary>
/// <typeparam name="T">The component type the parameters belong to.</typeparam>
public sealed class ParameterBuilder<T>
    where T : IComponent
{
    /// <summary>The accumulated parameter values keyed by component property name.</summary>
    private readonly Dictionary<string, object?> _parameters = [];

    /// <summary>Adds a parameter identified by a property-access expression.</summary>
    /// <typeparam name="TValue">The parameter value type.</typeparam>
    /// <param name="parameterSelector">A property-access expression identifying the parameter, e.g. <c>p =&gt; p.ViewModel</c>.</param>
    /// <param name="value">The value to supply for the parameter.</param>
    /// <returns>The same builder, for chaining.</returns>
    public ParameterBuilder<T> Add<TValue>(Expression<Func<T, TValue>> parameterSelector, TValue value)
    {
        ArgumentNullException.ThrowIfNull(parameterSelector);

        var body = parameterSelector.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } convert)
        {
            body = convert.Operand;
        }

        if (body is not MemberExpression member)
        {
            throw new ArgumentException("The parameter selector must be a property access expression.", nameof(parameterSelector));
        }

        _parameters[member.Member.Name] = value;
        return this;
    }

    /// <summary>Builds the accumulated parameters into a <see cref="ParameterView"/>.</summary>
    /// <returns>The parameter view supplied to the component.</returns>
    internal ParameterView Build() => ParameterView.FromDictionary(_parameters);
}
