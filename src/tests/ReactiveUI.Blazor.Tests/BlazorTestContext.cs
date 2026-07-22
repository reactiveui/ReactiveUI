// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// A minimal Blazor rendering host for component tests. Configure <see cref="Services"/> if the component needs
/// dependency injection, then call <see cref="RenderAsync{T}"/> to instantiate and render a component through the
/// real Blazor lifecycle (initialization, parameter application, after-render callbacks and disposal).
/// </summary>
/// <remarks>This hosts components on a single dispatcher using <see cref="BlazorTestRenderer"/> so the reactive
/// component base classes exercise their real render and activation paths.</remarks>
public class BlazorTestContext : IDisposable
{
    /// <summary>The service collection used to build the component service provider.</summary>
    private readonly ServiceCollection _services = new();

    /// <summary>The service provider built from <see cref="_services"/> on the first render.</summary>
    private IServiceProvider? _serviceProvider;

    /// <summary>The renderer that hosts the components, created lazily on the first render.</summary>
    private BlazorTestRenderer? _renderer;

    /// <summary>Indicates whether this context has been disposed.</summary>
    private bool _disposed;

    /// <summary>Gets the service collection used to build the component service provider. Configure it before the first render.</summary>
    protected IServiceCollection Services => _services;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Renders <typeparamref name="T"/> as a root component, applying the supplied parameters.</summary>
    /// <typeparam name="T">The component type to render.</typeparam>
    /// <param name="configureParameters">A callback that adds component parameters before the first render. Pass an empty callback to render without parameters.</param>
    /// <returns>A handle to the rendered component.</returns>
    protected Task<RenderedComponent<T>> RenderAsync<T>(Action<ParameterBuilder<T>> configureParameters)
        where T : IComponent
    {
        ArgumentNullException.ThrowIfNull(configureParameters);

        if (_renderer is null)
        {
            _serviceProvider = _services.BuildServiceProvider();
            _renderer = new(_serviceProvider, NullLoggerFactory.Instance);
        }

        var builder = new ParameterBuilder<T>();
        configureParameters(builder);
        return _renderer.RenderAsync(builder);
    }

    /// <summary>Releases the renderer and service provider.</summary>
    /// <param name="disposing"><see langword="true"/> to release managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _renderer?.Dispose();
            (_serviceProvider as IDisposable)?.Dispose();
        }

        _disposed = true;
    }
}
