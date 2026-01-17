// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>
///     A collection model.
/// </summary>
public class FakeCollectionModel : ReactiveObject
{
    private bool _isHidden;

    private int _someNumber;

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is hidden.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
    /// </value>
    public bool IsHidden
    {
        get => _isHidden;
        set => this.RaiseAndSetIfChanged(ref _isHidden, value);
    }

    /// <summary>
    ///     Gets or sets some number.
    /// </summary>
    public int SomeNumber
    {
        get => _someNumber;
        set => this.RaiseAndSetIfChanged(ref _someNumber, value);
    }
}
