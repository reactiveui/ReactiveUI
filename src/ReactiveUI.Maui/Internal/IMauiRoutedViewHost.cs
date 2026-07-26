// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui.Internal;
#else
namespace ReactiveUI.Maui.Internal;
#endif

/// <summary>Supplies the host-specific operations used to initialize a WinUI routed host.</summary>
internal interface IMauiRoutedViewHost : IEnableLogger
{
    /// <summary>Gets the router.</summary>
    RoutingState Router { get; }

    /// <summary>Gets or sets the view-contract observable.</summary>
    IObservable<string?> ViewContractObservable { get; set; }

    /// <summary>Gets the current view contract.</summary>
    string? ViewContract { get; }

    /// <summary>Stores the latest observed view contract without replacing its source observable.</summary>
    /// <param name="contract">The observed contract.</param>
    void SetObservedViewContract(string? contract);
}
