// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>
/// A view that implements only the non-generic <see cref="IViewFor"/>, so the source generator writes no lookup entry
/// for it and a host can only find it through a <c>Map</c> registration or a locator of its own.
/// </summary>
public class MappedOnlyView : UserControl, IViewFor
{
    /// <inheritdoc/>
    public object? ViewModel { get; set; }
}
