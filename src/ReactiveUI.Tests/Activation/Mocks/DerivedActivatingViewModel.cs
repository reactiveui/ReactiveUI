// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Activation.Mocks;

/// <summary>
/// A activating view model which is derived from another ActivatingViewModel.
/// </summary>
public class DerivedActivatingViewModel : ActivatingViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DerivedActivatingViewModel"/> class.
    /// </summary>
    public DerivedActivatingViewModel() =>
        this.WhenActivated(d =>
        {
            IsActiveCountAlso++;
            d(Disposable.Create(() => IsActiveCountAlso--));
        });

    /// <summary>
    /// Gets or sets the active count.
    /// </summary>
    public int IsActiveCountAlso { get; protected set; }
}
