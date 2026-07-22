// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Activation;

/// <summary>A activating view model which is derived from another ActivatingViewModel.</summary>
public class DerivedActivatingViewModel : ActivatingViewModel
{
    /// <summary>Initializes a new instance of the <see cref="DerivedActivatingViewModel" /> class.</summary>
    [SuppressMessage(
        "Performance",
        "PSH1011:Anonymous function captures state",
        Justification = "The deactivation callback decrements this fixture's own counter; capturing the instance is the intended activation pattern.")]
    public DerivedActivatingViewModel() =>
        this.WhenActivated(d =>
        {
            IsActiveCountAlso++;
            d(Scope.Create(() => IsActiveCountAlso--));
        });

    /// <summary>Gets the active count.</summary>
    public int IsActiveCountAlso { get; protected set; }
}
