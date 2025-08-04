// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Extension methods for ReactiveUI Builder functionality.
/// </summary>
public static class ReactiveUIBuilderExtensions
{
    /// <summary>
    /// Creates a builder for configuring ReactiveUI without using reflection.
    /// This provides an AOT-compatible alternative to the reflection-based InitializeReactiveUI method.
    /// </summary>
    /// <param name="resolver">The dependency resolver to configure.</param>
    /// <returns>A ReactiveUIBuilder instance for fluent configuration.</returns>
    public static Builder.ReactiveUIBuilder CreateBuilder(this IMutableDependencyResolver resolver)
    {
        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));
        return new Builder.ReactiveUIBuilder(resolver);
    }
}
