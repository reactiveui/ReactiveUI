// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A test <see cref="ISite"/> that always reports design mode.</summary>
internal sealed class DesignModeSite : ISite, IDisposable
{
    /// <inheritdoc/>
    public IComponent Component { get; } = new Component();

    /// <inheritdoc/>
    public IContainer? Container => null;

    /// <inheritdoc/>
    public bool DesignMode => true;

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc/>
    public void Dispose() => Component.Dispose();

    /// <inheritdoc/>
    public object? GetService(Type serviceType) => null;
}
