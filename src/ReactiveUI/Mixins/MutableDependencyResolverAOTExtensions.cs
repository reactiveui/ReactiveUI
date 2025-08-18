// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// AOT-friendly generic registration helpers for IMutableDependencyResolver.
/// These avoid reflection by relying on generic constraints and parameterless constructors.
/// </summary>
internal static class MutableDependencyResolverAOTExtensions
{
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Generic registration does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Members annotated with 'RequiresDynamicCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Generic registration does not use dynamic code")]
#endif
    internal static IMutableDependencyResolver RegisterViewForViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));
        resolver.Register(() => new TView(), typeof(IViewFor<TViewModel>), contract ?? string.Empty);
        return resolver;
    }

#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Generic registration does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Members annotated with 'RequiresDynamicCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Generic registration does not use dynamic code")]
#endif
    internal static IMutableDependencyResolver RegisterSingletonViewForViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));
        resolver.RegisterLazySingleton(() => new TView(), typeof(IViewFor<TViewModel>), contract ?? string.Empty);
        return resolver;
    }
}
