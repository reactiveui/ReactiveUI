// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>A <see cref="RoutedViewHost"/> that also finds views registered only with the service locator.</summary>
/// <remarks>
/// <para>
/// <see cref="RoutedViewHost"/> asks the view locator's generated view lookup and its <c>Map</c> registrations. This
/// twin asks those first, then the service locator for <see cref="IViewFor{T}"/> closed over the view model's
/// run-time type.
/// </para>
/// <para>
/// Closing a generic type at run time needs code the compiler never generated, so this type is not safe to compile
/// ahead of time. Use <see cref="RoutedViewHost"/> when you can.
/// </para>
/// </remarks>
[RequiresDynamicCode(ViewResolutionMessages.UnsafeRoutedHost)]
[DebuggerDisplay("{Router}, {ViewLocator}")]
public class RoutedViewHostUnsafe : RoutedViewHost
{
    /// <summary>Initializes a new instance of the <see cref="RoutedViewHostUnsafe"/> class.</summary>
    public RoutedViewHostUnsafe()
        : base(static (viewLocator, viewModel, contract) => viewLocator.ResolveViewUnsafe(viewModel, contract))
    {
    }
}
