// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>
/// A mock list item view model used by binding tests.
/// </summary>
public class MockBindListItemViewModel : ReactiveUI.ReactiveObject
{
    /// <summary>
    /// Backing field for the <see cref="Name"/> property.
    /// </summary>
    private string _name = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockBindListItemViewModel"/> class.
    /// </summary>
    /// <param name="name">The display name of the item.</param>
    public MockBindListItemViewModel(string name) => Name = name;

    /// <summary>
    /// Gets or sets displayed name of the crumb.
    /// </summary>
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
