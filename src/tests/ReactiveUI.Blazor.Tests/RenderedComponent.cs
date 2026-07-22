// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;

namespace ReactiveUI.Blazor.Tests;

/// <summary>A handle to a component rendered by <see cref="BlazorTestContext"/>.</summary>
/// <typeparam name="T">The rendered component type.</typeparam>
public sealed class RenderedComponent<T>
    where T : IComponent
{
    /// <summary>The renderer that produced and continues to host the component.</summary>
    private readonly BlazorTestRenderer _renderer;

    /// <summary>Initializes a new instance of the <see cref="RenderedComponent{T}"/> class.</summary>
    /// <param name="renderer">The renderer that produced the component.</param>
    /// <param name="instance">The rendered component instance.</param>
    internal RenderedComponent(BlazorTestRenderer renderer, T instance)
    {
        _renderer = renderer;
        Instance = instance;
    }

    /// <summary>Gets the rendered component instance.</summary>
    public T Instance { get; }

    /// <summary>Re-renders the component with updated parameters.</summary>
    /// <param name="configureParameters">A callback that supplies the new parameters.</param>
    /// <returns>A task that completes when the re-render has settled.</returns>
    public Task RenderAsync(Action<ParameterBuilder<T>> configureParameters)
    {
        ArgumentNullException.ThrowIfNull(configureParameters);

        var builder = new ParameterBuilder<T>();
        configureParameters(builder);
        return _renderer.SetParametersAsync(Instance, builder.Build());
    }
}
