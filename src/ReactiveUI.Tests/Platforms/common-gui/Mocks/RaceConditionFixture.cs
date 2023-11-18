// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// A fixture for demonstrating race conditions.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
public class RaceConditionFixture : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<bool> _A;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaceConditionFixture"/> class.
    /// </summary>
    public RaceConditionFixture()
    {
        // We need to generate a value on subscription
        // which is different than the default value.
        // This triggers the property change firing
        // upon subscription in the ObservableAsPropertyHelper
        // constructor.
        Observables.True.Do(_ => Count++).ToProperty(this, x => x.A, out _A);
    }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="RaceConditionFixture"/> is a.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a; otherwise, <c>false</c>.
    /// </value>
    public bool A => _A.Value;
}
