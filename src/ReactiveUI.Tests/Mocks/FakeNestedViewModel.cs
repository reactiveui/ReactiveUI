// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// A fake nested view model.
/// </summary>
public class FakeNestedViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeNestedViewModel"/> class.
    /// </summary>
    public FakeNestedViewModel() => NestedCommand = ReactiveCommand.Create(static () => { });

    /// <summary>
    /// Gets or sets the nested command.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NestedCommand { get; protected set; }
}
