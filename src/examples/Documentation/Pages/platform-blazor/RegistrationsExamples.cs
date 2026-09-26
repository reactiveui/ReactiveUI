// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>
/// Shows what <see cref="Blazor.Registrations"/> adds to a resolver, and what <see cref="Blazor.PlatformOperations"/>
/// reports. ReactiveUI's own root namespace also declares a <see cref="Registrations"/>, and that one wins ordinary name
/// lookup even with a <c>using ReactiveUI.Blazor;</c> directive in scope, so this page always qualifies the Blazor ones.
/// </summary>
public static class RegistrationsExamples
{
    /// <summary>
    /// <c>Registrations.Register</c> adds Blazor's <see cref="IPlatformOperations"/> and its binding type converters, such
    /// as <see cref="IntegerToStringTypeConverter"/>, to any <see cref="IRegistrar"/>.
    /// </summary>
    public static void RegisterBlazorServices()
    {
        ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        Blazor.Registrations registrations = new();

        registrations.Register(registrar);

        IPlatformOperations? platformOperations = resolver.GetService<IPlatformOperations>();
        IntegerToStringTypeConverter? integerConverter = resolver.GetServices<IBindingTypeConverter>()
            .OfType<IntegerToStringTypeConverter>()
            .FirstOrDefault();

        Console.WriteLine(platformOperations is Blazor.PlatformOperations);
        Console.WriteLine(integerConverter is not null);

        // Output:
        // True
        // True
    }

    /// <summary><see cref="Blazor.PlatformOperations.GetOrientation"/> always returns <see langword="null"/> on Blazor.</summary>
    public static void ReadTheOrientation()
    {
        Blazor.PlatformOperations platformOperations = new();

        string? orientation = platformOperations.GetOrientation();

        Console.WriteLine(orientation is null);

        // Output:
        // True
    }
}
