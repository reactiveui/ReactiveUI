// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;

namespace ReactiveUI.Blazor.Tests;

/// <summary>A single-dispatcher <see cref="Renderer"/> that hosts components for testing and discards display output.</summary>
public sealed class BlazorTestRenderer : Renderer
{
    /// <summary>Initializes a new instance of the <see cref="BlazorTestRenderer"/> class.</summary>
    /// <param name="serviceProvider">The service provider used for component activation and injection.</param>
    /// <param name="loggerFactory">The logger factory required by the base renderer.</param>
    public BlazorTestRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        : base(serviceProvider, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

    /// <summary>Instantiates and renders a root component with the supplied parameters.</summary>
    /// <typeparam name="T">The component type to render.</typeparam>
    /// <param name="builder">The builder describing the initial parameters.</param>
    /// <returns>A handle to the rendered component once the initial render has settled.</returns>
    public async Task<RenderedComponent<T>> RenderAsync<T>(ParameterBuilder<T> builder)
        where T : IComponent
    {
        ArgumentNullException.ThrowIfNull(builder);

        var parameters = builder.Build();
        var component = await Dispatcher.InvokeAsync(async () =>
        {
            var instance = (T)InstantiateComponent(typeof(T));
            var componentId = AssignRootComponentId(instance);
            await RenderRootComponentAsync(componentId, parameters).ConfigureAwait(true);
            return instance;
        }).ConfigureAwait(true);

        // OnAfterRender(firstRender: true) wires PropertyChanged and schedules a single StateHasChanged (the
        // second render). Draining the dispatcher once lets that queued render run before the handle is returned,
        // so callers observe the settled render count without polling.
        await Dispatcher.InvokeAsync(static () => { }).ConfigureAwait(true);

        return new(this, component);
    }

    /// <summary>Applies new parameters to an already-rendered root component and re-renders it.</summary>
    /// <param name="component">The component to update.</param>
    /// <param name="parameters">The new parameters.</param>
    /// <returns>A task that completes when the re-render has settled.</returns>
    public async Task SetParametersAsync(IComponent component, ParameterView parameters)
    {
        ArgumentNullException.ThrowIfNull(component);

        await Dispatcher.InvokeAsync(() => component.SetParametersAsync(parameters)).ConfigureAwait(true);
        await Dispatcher.InvokeAsync(static () => { }).ConfigureAwait(true);
    }

    /// <inheritdoc/>
    protected override void HandleException(Exception exception) =>
        ExceptionDispatchInfo.Capture(exception).Throw();

    /// <inheritdoc/>
    protected override Task UpdateDisplayAsync(in RenderBatch renderBatch) => Task.CompletedTask;
}
