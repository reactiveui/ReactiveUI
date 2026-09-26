// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows <see cref="Winforms.Registrations"/>, the module that <c>WithWinForms</c> loads. It registers WinForms's
/// platform operations, activation fetcher, two <see cref="ISetMethodBindingConverter"/>s (for a <see cref="Panel"/>
/// and a <see cref="TableLayoutPanel"/>) and four <see cref="IBindingTypeConverter"/>s, into an
/// <see cref="IRegistrar"/> that forwards to a fresh Splat resolver, so this example never touches the app's real
/// registrations. Qualified as <c>Winforms.Registrations</c>: this code's own namespace nests under
/// <c>ReactiveUI</c>, which also carries its own core <c>Registrations</c> type, so the unqualified name would bind
/// to that one instead.
/// </summary>
public static class RegistrationsExamples
{
    /// <summary>Registering the module fills a resolver with every service WinForms binding needs.</summary>
    public static void RegisterPlatformServices()
    {
        ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);

        new Winforms.Registrations().Register(registrar);

        Console.WriteLine(resolver.GetService<IPlatformOperations>()?.GetOrientation() ?? "(null)");
        Console.WriteLine(resolver.GetServices<ISetMethodBindingConverter>().Count());
        Console.WriteLine(resolver.GetServices<IBindingTypeConverter>().Count());

        // Output:
        // (null)
        // 2
        // 4
    }
}
