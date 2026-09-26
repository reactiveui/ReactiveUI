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

/// <summary>A <see cref="ViewModelViewHost"/> that also finds views known only by the view model's run-time type.</summary>
/// <remarks>
/// <para>
/// <see cref="ViewModelViewHost"/> uses the view lookup the source generator writes. This twin asks the view locator
/// with the view model as an <see cref="object"/>: the generated lookup first, then the locator's explicit mappings,
/// then the service locator for <see cref="IViewFor{T}"/> closed over the view model's run-time type.
/// </para>
/// <para>
/// Closing a generic type at run time needs code the compiler never generated, so this type is not safe to compile
/// ahead of time. Use <see cref="ViewModelViewHost"/> or <see cref="ViewModelViewHost{TViewModel}"/> when you can.
/// </para>
/// </remarks>
[RequiresDynamicCode(ViewResolutionMessages.UnsafeHost)]
[DebuggerDisplay("{ViewContractObservable}, {DefaultContent}")]
public partial class ViewModelViewHostUnsafe : ViewModelViewHost
{
    /// <summary>Initializes a new instance of the <see cref="ViewModelViewHostUnsafe"/> class.</summary>
    public ViewModelViewHostUnsafe()
        : base(static (viewLocator, viewModel, contract) => viewLocator.ResolveView(viewModel, contract))
    {
    }
}
