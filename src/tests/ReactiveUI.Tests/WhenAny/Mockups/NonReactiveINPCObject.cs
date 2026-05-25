// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny.Mockups;

/// <summary>
///     An object that implements <see cref="INotifyPropertyChanged" /> without deriving from ReactiveObject.
/// </summary>
public class NonReactiveInpcObject : INotifyPropertyChanged
{
    private TestFixture _inpcProperty = new();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Gets or sets the INPC property.
    /// </summary>
    public TestFixture InpcProperty
    {
        get => _inpcProperty;
        set
        {
            if (_inpcProperty == value)
            {
                return;
            }

            _inpcProperty = value;

            PropertyChanged?.Invoke(this, new(nameof(InpcProperty)));
        }
    }
}
