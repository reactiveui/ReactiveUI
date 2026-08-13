// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>The view registered for <see cref="RoutedTestViewModel"/> under a contract.</summary>
public class ContractRoutedTestView : IViewFor<RoutedTestViewModel>
{
    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (RoutedTestViewModel?)value;
    }

    /// <inheritdoc/>
    public RoutedTestViewModel? ViewModel { get; set; }
}
