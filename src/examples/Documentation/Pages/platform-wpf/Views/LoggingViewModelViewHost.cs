// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// A <see cref="ViewModelViewHost"/> that logs every view it resolves, by overriding
/// <see cref="ViewModelViewHost.ResolveViewForViewModel(object?, string?)"/>. <see cref="CourseListView"/> uses it
/// for its student summary panel.
/// </summary>
[System.Diagnostics.DebuggerDisplay("LoggingViewModelViewHost")]
public sealed class LoggingViewModelViewHost : ViewModelViewHost
{
    /// <inheritdoc/>
    protected override void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        base.ResolveViewForViewModel(viewModel, contract);
        Console.WriteLine($"Summary panel resolved: {(viewModel is null ? "(none)" : Content?.GetType().Name)}");
    }
}
