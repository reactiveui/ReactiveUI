// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Activation;

/// <summary>Simulates a activating view model.</summary>
public class ActivatingViewModel : ReactiveObject, IActivatableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="ActivatingViewModel" /> class.</summary>
    [SuppressMessage(
        "Performance",
        "PSH1011:Anonymous function captures state",
        Justification = "The deactivation callback decrements this fixture's own counter; capturing the instance is the intended activation pattern.")]
    public ActivatingViewModel()
    {
        Activator = new();

        this.WhenActivated(d =>
        {
            IsActiveCount++;
            d(Scope.Create(() => IsActiveCount--));
        });
    }

    /// <summary>Gets the Activator which will be used by the View when Activation/Deactivation occurs.</summary>
    public ViewModelActivator Activator { get; protected set; }

    /// <summary>Gets the active count.</summary>
    public int IsActiveCount { get; protected set; }
}
