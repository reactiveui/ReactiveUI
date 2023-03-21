// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests;

/// <summary>
/// A test model for the ObservabeAsPropertyAttribute.
/// </summary>
public class ObservableAsTestModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsTestModel"/> class.
    /// </summary>
    public ObservableAsTestModel() => Observable.Return("foo").ToPropertyEx(this, x => x.TestProperty);

    /// <summary>
    /// Gets the test property which will reference our generated observable.
    /// </summary>
    [ObservableAsProperty]
    public string? TestProperty { get; private set; }
}
